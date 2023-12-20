using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using System.Xml;
using AsterixDecoder.Data_Items_Objects;
using IronXL;

namespace AsterixDecoder
{
    public partial class MainWindow : Form
    {

        //All the CAT048 data units decoded
        List<CAT048> elements = new List<CAT048>();

        //Aircrafts in P3 information
        List<ACinfo> aircraftsInfo;
        Dictionary<string, List<ACinfo>> aircraftsInfoDic;

        //Take offs in P3 information
        List<Ruta> despeguesList;

        //AC classification P3 information
        ACclassification ACclassification;


        //Simulation
        //-----------------------------------------------------------

        //For a given AC_ID all the CAT048 objects (data items with the necessary info) associated to it in temporal order
        Dictionary<string, List<CAT048_simulation>> aircraftsList;
        //For a given AC_ID the GMapMarker associated to it
        public Dictionary<string, RotatableMarker> aircraftsMarkers;
        //Markers in the map
        GMapOverlay markers;
        //AC icon global for all
        Bitmap ACicon;
        int ACiconSize;

        bool playing;
        DateTime simulationTime;

        //-----------------------------------------------------------
        string projectDirectory;

        UsefulFunctions usefulFunctions = new UsefulFunctions();

        bool firstTimeSimOpened;

        bool isSimulating;

        bool endedSimulation;

        // Resize variables
        private int YourInitialFormWidth;
        private int YourInitialFormHeight;

        public MainWindow()
        {
            InitializeComponent();

            // Store the initial form size when the form is loaded
            YourInitialFormWidth = this.Width;
            YourInitialFormHeight = this.Height;

            this.Resize += MainWindow_Resize;

            viewPanel.Visible = false;
            progressBar1.Visible = false;
            progressLbl.Visible = false;
            gMapControl1.Visible = true;
            infoLbl.Visible = false;
            firstTimeSimOpened = true;

            //Logo
            this.projectDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string filePath = Path.Combine(projectDirectory, "img/logo.png");
            pictureBoxLogo.LoadAsync(filePath);
            pictureBoxLogo.SizeMode = PictureBoxSizeMode.StretchImage;

            //Logo Label
            title.Font = new Font("Cascadia Code", 16);
            title.Text = "AsTeRiX DeCoDeR";
            title.TextAlign = ContentAlignment.TopCenter;
            title.BackColor = Color.Black;            
                        

            //Expand screen
            this.TopMost = true;
            this.WindowState = FormWindowState.Normal;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;

            
        }

        private void MainWindow_Resize(object sender, EventArgs e)
        {
            ResizeAllElements();
        }

        private void ResizeAllElements()
        {
            foreach (Control control in this.Controls)
            {
                if (control.Name != "flowLayoutPanel1")
                {
                    // Adjust the size and position of each control based on the form size

                    // Calculate the new size and position based on the form size
                    float scaleX = (float)this.Width / (float)YourInitialFormWidth;
                    float scaleY = (float)this.Height / (float)YourInitialFormHeight;

                    int newX = (int)(control.Location.X * scaleX);
                    int newY = (int)(control.Location.Y * scaleY);
                    int newWidth = (int)(control.Width * scaleX);
                    int newHeight = (int)(control.Height * scaleY);

                    // Set the new size and position
                    control.Location = new Point(newX, newY);
                    control.Size = new Size(newWidth, newHeight);
                }
                
            }

            YourInitialFormWidth = this.Width;
            YourInitialFormHeight = this.Height;

        }


        

        //Decoding to CSV
        private void ReportProgess(int percent)
        {
            
            progressLbl.Text = "Loading... " + Convert.ToString(percent) + " %";
            progressLbl.Refresh();
            progressBar1.Value = percent;

            if (percent == 100)
            {
                progressLbl.Visible = false;
                progressBar1.Visible = false;
            }
        }


        //Reading a CSV file
        public void readCSV(string filePath)
        {
            string[] lines = System.IO.File.ReadAllLines(filePath);
            if (lines.Length > 0)
            {
                this.aircraftsInfo = new List<ACinfo>();
                this.aircraftsInfoDic = new Dictionary<string, List<ACinfo>>();

                progressLbl.Visible = true;
                progressBar1.Visible = true;

                //first line dissmissed because it is the header
                int len = lines.Length;
                for (int i = 1; i < len; i++)
                {
                    int valProgress = Convert.ToInt32(Convert.ToDouble(i) / len * 100);
                    ReportProgess(valProgress);

                    string[] row = Regex.Split(lines[i], ";");

                    ACinfo ac = new ACinfo(row);

                    aircraftsInfo.Add(ac);
                    Console.WriteLine(ac.I161_Tn);

                    //If the dictionary already has the key
                    if (this.aircraftsInfoDic.ContainsKey(ac.I161_Tn))
                    {
                        this.aircraftsInfoDic[ac.I161_Tn].Add(ac);
                    }
                    else
                    {
                        List<ACinfo> list = new List<ACinfo>();
                        list.Add(ac);
                        this.aircraftsInfoDic.Add(ac.I161_Tn, list);
                    }
                }
                popUpLabel(Convert.ToString(aircraftsInfo.Count));
            }
            
            
        }

        //Decoding to CSV
        private void cSVToolboxToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Hide simulation stuff
            viewHideSimulation(false);

            string imagePath = Path.Combine(this.projectDirectory, "img/playButton.png");
            playPictureBox.Image = new Bitmap(imagePath);
            this.playing = false;

        }


        public void viewHideSimulation(bool visible)
        {
            viewPanel.Visible = visible;
            gMapControl1.Visible = visible;
        }

        public void popUpLabel(string text)
        {
            var t = new Timer();
            infoLbl.Text = text;
            infoLbl.Visible = true;
            t.Interval = 3000; // it will Tick in 3 seconds
            t.Tick += (sender, e) =>
            {
                infoLbl.Hide();
                t.Stop();
            };
            t.Start();
        }


        ///-----------------------------------------SIMULATION-----------------------------------------------

        public void viewHideDecoder(bool visible)
        {
            
        }

        //To generate the map from the elements decoded.
        private void trajectoriesSimulatorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            hourBox.ReadOnly = true;

            //Hide all the other forms
            viewHideDecoder(false);

            //View the simulator keyboard
            viewPanel.Visible = true;

            this.endedSimulation = false;

            if (firstTimeSimOpened)
            {
                showTime();
                
                hourBox.TextAlign = HorizontalAlignment.Center;

                //Keyboard            
                //Play button
                string imagePath0 = Path.Combine(this.projectDirectory, "img/playButton.png");
                playPictureBox.Image = new Bitmap(imagePath0);
                playPictureBox.SizeMode = PictureBoxSizeMode.AutoSize;

                //Back button
                string imagePath2 = Path.Combine(this.projectDirectory, "img/backButton.png");
                backPictureBox.Image = new Bitmap(imagePath2);
                backPictureBox.SizeMode = PictureBoxSizeMode.AutoSize;

                //Foward button
                string imagePath3 = Path.Combine(this.projectDirectory, "img/fowardButton.png");
                fowardPictureBox.Image = new Bitmap(imagePath3);
                fowardPictureBox.SizeMode = PictureBoxSizeMode.AutoSize;

                //Markers overlayed in the map
                markers = new GMapOverlay("markers");
                this.ACiconSize = 30;
                string iconPath = Path.Combine(this.projectDirectory, "img/aircraft_icon.png");
                ACicon = new Bitmap(new Bitmap(iconPath), new Size(this.ACiconSize, this.ACiconSize));

                //Gmap
                gMapControl1.MapProvider = GMapProviders.GoogleMap;
                gMapControl1.Position = new PointLatLng(41.298, 2.080);
                gMapControl1.MinZoom = 2;
                gMapControl1.MaxZoom = 24;
                gMapControl1.Zoom = 9;
                gMapControl1.AutoScroll = true;
                gMapControl1.DragButton = MouseButtons.Left;
                gMapControl1.CanDragMap = true;

                IsMapas.SelectedItem = "Google Maps Callejero";

                Controls.Add(gMapControl1);

                speedDecisionBox.SelectedItem = "x 1";
                speedDecisionBox.DropDownStyle = ComboBoxStyle.DropDownList;

                firstTimeSimOpened = false;
            }
            else
            {
                string imagePath = Path.Combine(this.projectDirectory, "img/playButton.png");
                playPictureBox.Image = new Bitmap(imagePath);
                this.playing = false;

                timer1.Stop();
            }

            gMapControl1.Visible = true;
        }

        public void showTime()
        {
            if (elements.Count != 0)
            {                
                string t = usefulFunctions.FormatTimeString(elements[0].UTCTime);
                this.simulationTime = DateTime.ParseExact(t, "HH:mm:ss", null);
                hourBox.Text = t;
            }
            else
            {
                this.simulationTime = DateTime.ParseExact("08:00:00", "HH:mm:ss", null);
                hourBox.Text = "08:00:00";

            }

        }

        public void showEndTime()
        {
            if (elements.Count != 0 )
            {
                string et = usefulFunctions.FormatTimeString(elements[elements.Count - 1].UTCTime);
                this.simulationTime = DateTime.ParseExact(et, "HH:mm:ss", null);
                string pt = (this.simulationTime.AddSeconds(1).ToString());
                string jt = pt.Split(' ')[1];

                if (Convert.ToInt32(jt.Split(':')[0]) < 10)
                {
                    jt = '0' + jt;
                }

                this.simulationTime = DateTime.ParseExact(jt, "HH:mm:ss", null); 
                hourBox.Text = jt;
            }
            else
            {
                this.simulationTime = DateTime.ParseExact("00:00:00", "HH:mm:ss", null);
                hourBox.Text = "00:00:00";

                popUpLabel("🛠 End simulation button.");
            }

            
        }

        private void visualizeCurrentElements()
        {
            //Simulation
            this.playing = false;
            if (isSimulating)
            {
                try
                {
                    //Generate the dictionary with each AC_ID and its list of data items associated
                    generateACDictionaryFromElements();

                    gMapControl1.Visible = true;

                    //Generate the dictionary with the AC_ID and its associated marker
                    aircraftsMarkers = new Dictionary<string, RotatableMarker>();

                    foreach (string id in aircraftsList.Keys)
                    {
                        plotTheCurrentDataItemForAGivenTime(id);
                    }

                    //First you generate the markers and then you put it in the map
                    gMapControl1.Overlays.Add(markers);

                }
                catch
                {
                    popUpLabel("❌ Something went wrong...");
                }

                this.isSimulating = false;
            }
                         
            
        }

        private void generateACDictionaryFromElements()
        {
            aircraftsList = new Dictionary<string, List<CAT048_simulation>>();

            int num = 0;
            int numlines = elements.Count;

            foreach (CAT048 rawElement in elements)
            {
                CAT048_simulation aircraft = new CAT048_simulation(rawElement);

                //if (aircraft.AC_ID != "N/A")
                //{
                    int valProgress = Convert.ToInt32(Convert.ToDouble(num) / numlines * 100);
                    ReportProgess(valProgress);
                    num++;

                    // Check if the AC_ID is already in
                    if (aircraftsList.ContainsKey(aircraft.AC_ID))
                    {
                        // Add the element to the existing list
                        aircraftsList[aircraft.AC_ID].Add(aircraft);
                    }
                    else
                    {
                        // If the category doesn't exist, create a new category with the element
                        aircraftsList.Add(aircraft.AC_ID, new List<CAT048_simulation> { aircraft });
                    }
                //}

                
            }
        }       

        //Simulator
        private void playPictureBox_Click(object sender, EventArgs e)
        {
            if (elements.Count != 0 && aircraftsList != null)
            {
                if (this.endedSimulation)
                {
                    this.playing = true;
                }
                //If it is playing and you click
                if (playing)
                {
                    //Not playing any more
                    this.playing = !playing;

                    //Now the button tells you to play it again
                    string imagePath = Path.Combine(this.projectDirectory, "img/playButton.png");

                    playPictureBox.Image = new Bitmap(imagePath);

                    timer1.Stop();
                }
                else
                {
                    this.playing = !playing;
                    //Play button
                    string imagePath = Path.Combine(this.projectDirectory, "img/pauseButton.png");
                    playPictureBox.Image = new Bitmap(imagePath);

                    //Set timer interval
                    speedDecisionBox_SelectedIndexChanged(sender, e);

                    timer1.Start();

                    //Set time
                    string simTime = hourBox.Text;
                    this.simulationTime = DateTime.ParseExact(simTime, "HH:mm:ss", null);

                }
            }
            else
            {
                popUpLabel("🛠 Unavailable! You need to 'Trajectories simulator > Simulate' \nafter decoding the desired binary file.");
            }

        }

        //Simulator

        // Code to be executed on each timer tick
        private void timer1_Tick(object sender, EventArgs e)
        {
            //Set time in case the our is changed manually
            string simTime = hourBox.Text;
            this.simulationTime = DateTime.ParseExact(simTime, "HH:mm:ss", null);

            //Change hour
            this.simulationTime = this.simulationTime.AddSeconds(1); //you just add one second
            hourBox.Text = this.simulationTime.TimeOfDay.ToString();
            hourBox.Refresh();

            foreach (string id in aircraftsList.Keys)
            {
                plotTheCurrentDataItemForAGivenTime(id);
            }

            if (markers.Markers.Count == 0)
            {
                timer1.Stop();
                popUpLabel("✅ End of simulation");
            }

        }

        //Save the current simulated flights into a KML
        private void saveTrajectoriesInKMLFormatToolStripMenuItem_Click(object sender, EventArgs e)
        {

            if (elements.Count != 0 && aircraftsList!= null)
            {             
                // Create a SaveFileDialog
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "KML files (*.kml)|*.kml|All files (*.*)|*.*";
                saveFileDialog.Title = "Save KML File";

                // Show the dialog and get the user's response
                DialogResult result = saveFileDialog.ShowDialog();

                // If the user clicked OK
                if (result == DialogResult.OK)
                {
                    // Get the selected file name and path
                    string kmlFilePath = saveFileDialog.FileName;

                    // Generate a KML file
                    var settings = new XmlWriterSettings
                    {
                        Indent = true
                    };

                    // Random number generator for colors
                    Random rand = new Random();
                    // Fixed alpha channel value (you can modify this if needed)
                    int alpha = 255;

                    using (var writer = XmlWriter.Create(kmlFilePath, settings))
                    {
                        writer.WriteStartElement("kml", "http://www.opengis.net/kml/2.2");
                        writer.WriteStartElement("Document");

                        foreach (string AC_ID in aircraftsList.Keys)
                        {
                            writer.WriteStartElement("Placemark");
                            writer.WriteElementString("name", AC_ID);
                            writer.WriteElementString("description", 
                                "Address - " + aircraftsList[AC_ID][0].AC_Adress + " - " +
                                "Mode 3A - " + aircraftsList[AC_ID][0].Mode_3A + " - " +
                                "TYP020 - " + aircraftsList[AC_ID][0].TYP);
                            writer.WriteStartElement("LineString");
                            writer.WriteElementString("altitudeMode", "absolute");
                            writer.WriteStartElement("coordinates");

                            foreach (CAT048_simulation item in aircraftsList[AC_ID])
                            {
                                writer.WriteString(item.Longitude.Replace(",", ".") + "," + item.Latitude.Replace(",", ".") + "," + item.Flight_Level.Replace(",", ".") + " ");
                            }

                            writer.WriteEndElement(); // End Coordinates
                            writer.WriteEndElement(); // End LineString

                            writer.WriteStartElement("Style");
                            writer.WriteStartElement("LineStyle");

                            int red = rand.Next(256);
                            int green = rand.Next(256);
                            int blue = rand.Next(256);

                            // Format the color in the 8-digit HEX format (with alpha channel)
                            string randomColor = $"#{alpha:x2}{red:x2}{green:x2}{blue:x2}";

                            writer.WriteElementString("color", randomColor);

                            writer.WriteEndElement(); // End Style
                            writer.WriteEndElement(); // End LineStyle

                            writer.WriteEndElement(); // End Placemark
                        }

                        writer.WriteEndElement(); // End Document
                        writer.WriteEndElement(); // End kml

                    }

                    popUpLabel("✅ KML file saved to: " + kmlFilePath);
                }

            }
            else
            {
                popUpLabel("❌ No KML to save, first decode and simulate.");
            }

        }

        //Change velocity
        private void speedDecisionBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedItem = speedDecisionBox.Items[speedDecisionBox.SelectedIndex].ToString();
            string factor = selectedItem.Split(' ')[1];
            int interval = Convert.ToInt32(1000.0m / Convert.ToDecimal(factor));
            
            if (factor == "0.5")
            {
                interval = 2000;
            }            

            timer1.Interval = interval;
        }


        private void plotTheCurrentDataItemForAGivenTime(string AC_ID)
        {
            bool found = false;
            int initialDataItem = 0;
            DateTime from = this.simulationTime;

            while (initialDataItem < aircraftsList[AC_ID].Count && !found)
            {                
                found = usefulFunctions.isDateBetween(from, from.AddSeconds(1), aircraftsList[AC_ID][initialDataItem].UTCTime); //next second

                if (found)
                {
                    double latt = Convert.ToDouble(aircraftsList[AC_ID][initialDataItem].Latitude);
                    double lonn = Convert.ToDouble(aircraftsList[AC_ID][initialDataItem].Longitude);

                    PointLatLng newPosition = new PointLatLng(latt, lonn);

                    if (aircraftsMarkers.ContainsKey(AC_ID))
                    {
                        aircraftsMarkers[AC_ID].Position = newPosition;
                        aircraftsMarkers[AC_ID].rotate(float.Parse(aircraftsList[AC_ID][initialDataItem].heading));
                    }
                    else
                    {
                        RotatableMarker marker2 = new RotatableMarker(newPosition, ACicon, float.Parse(aircraftsList[AC_ID][initialDataItem].heading), AC_ID);

                        //Associate the AC_ID to its marker
                        aircraftsMarkers.Add(AC_ID, marker2);

                        //markers.Markers.Add(marker);
                        markers.Markers.Add(marker2);
                    }
                }
                initialDataItem++;
            }

            if (!found)
            {
                bool aparecera = false;
                initialDataItem = 0;
                while (initialDataItem < aircraftsList[AC_ID].Count && !aparecera)
                {
                    aparecera = usefulFunctions.isDateBetween(from, from.AddSeconds(8), aircraftsList[AC_ID][initialDataItem].UTCTime);

                    initialDataItem++;
                }
                if (!aparecera)
                {
                    if (aircraftsMarkers.ContainsKey(AC_ID))
                    {
                        markers.Markers.Remove(aircraftsMarkers[AC_ID]);
                        aircraftsMarkers.Remove(AC_ID);
                    }
                }
                
            }

        }
        private void MainWindow_Load(object sender, EventArgs e)
        {
            

        }

        private void IsMapas_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (IsMapas.Text == "Google Maps Satélite")
                gMapControl1.MapProvider = GMapProviders.GoogleSatelliteMap;
            if (IsMapas.Text == "Google Maps Callejero")
                gMapControl1.MapProvider = GMapProviders.GoogleMap;
            if (IsMapas.Text == "Google Maps Híbrido")
                gMapControl1.MapProvider = GMapProviders.GoogleHybridMap;
            if (IsMapas.Text == "OpenStreetMap")
                gMapControl1.MapProvider = GMapProviders.OpenStreetMap;
            if (IsMapas.Text == "OpenCycleMap")
                gMapControl1.MapProvider = GMapProviders.OpenCycleMap;

            gMapControl1.Refresh();

        }

        private void backPictureBox_Click(object sender, EventArgs e)
        {
            showTime();

            this.playing = false;
            string imagePath = Path.Combine(this.projectDirectory, "img/playButton.png");
            playPictureBox.Image = new Bitmap(imagePath);
            timer1.Stop();

            if (elements.Count != 0 && aircraftsList != null)
            {
                foreach (string id in aircraftsList.Keys)
                {
                    plotTheCurrentDataItemForAGivenTime(id);
                }
            }
            else
            {
                popUpLabel("🛠 Restart simulation button.");
            }

            this.endedSimulation = false;

            gMapControl1.Refresh();
            
        }

        private void fowardPictureBox_Click(object sender, EventArgs e)
        {
            showEndTime();

            this.playing = false;
            string imagePath = Path.Combine(this.projectDirectory, "img/playButton.png");
            playPictureBox.Image = new Bitmap(imagePath);
            timer1.Stop();

            if (elements.Count != 0 && aircraftsList != null)
            {
                foreach (string id in aircraftsList.Keys)
                {
                    plotTheCurrentDataItemForAGivenTime(id);
                }

                popUpLabel("✅ End of simulation");
            }

            this.endedSimulation = true;

            gMapControl1.Refresh();

        }

        private void simulateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            trajectoriesSimulatorToolStripMenuItem_Click(sender, e);

            //Set initial time
            showTime();

            visualizeCurrentElements();
            if (elements.Count == 0)
            {
                popUpLabel("🔎 There is nothing to simulate, first decode.");
            }

            string imagePath = Path.Combine(this.projectDirectory, "img/playButton.png");
            playPictureBox.Image = new Bitmap(imagePath);
            this.playing = false;

        }

        private void importCSVFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Hide simulation stuff
            viewHideSimulation(false);

            try
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Filter = "CSV files (*.csv)|*.csv";
                using (ofd)
                {
                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        string path = ofd.FileName.ToString();

                        readCSV(path);

                        popUpLabel("⏳ Loaded " + Convert.ToString(aircraftsInfo.Count) + " aircraft items!");
                        popUpLabel("⏳ Loaded " + Convert.ToString(aircraftsInfoDic.Keys.Count) + " different aircrafts!");
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
                popUpLabel("❌ Something went wrong... Please, try again!");
            }

        }


        private void importTakeOffsExcelFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Excel Files|*.xls;*.xlsx";
                openFileDialog.Title = "Select an Excel File";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string selectedFilePath = openFileDialog.FileName;
                    LeerArchivoExcel(selectedFilePath);
                }
            }
        }

        private void LeerArchivoExcel(string rutaArchivo)
        {
            this.despeguesList = new List<Ruta>();
            try
            {
                WorkBook workBook = WorkBook.Load(rutaArchivo);
                WorkSheet workSheet = workBook.WorkSheets.First();

                // Iterate over all cells in the worsheet
                int row = 2;
                while (row <= workSheet.RowCount)
                {
                    Ruta despegue = new Ruta(
                        Convert.ToString(workSheet["A" + Convert.ToString(row)]),
                        Convert.ToString(workSheet["B" + Convert.ToString(row)]),
                        Convert.ToString(workSheet["C" + Convert.ToString(row)]),
                        Convert.ToString(workSheet["D" + Convert.ToString(row)]),
                        Convert.ToString(workSheet["E" + Convert.ToString(row)]),
                        Convert.ToString(workSheet["F" + Convert.ToString(row)]),
                        Convert.ToString(workSheet["G" + Convert.ToString(row)]),
                        Convert.ToString(workSheet["H" + Convert.ToString(row)])
                        );

                    if (despegue.id != null)
                    {                        
                        this.despeguesList.Add(despegue);
                    }
                    row++;
                }

                popUpLabel("✅ Correctly loaded " + Convert.ToString(despeguesList.Count)+ " take off!");
                                
            }
            catch (Exception ex)
            {
                popUpLabel("❌ Something went wrong... Is the EXCEL file open somewhere else?");
            }

        }

        private void importClassificationACsExcelFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                try
                {
                    openFileDialog.Filter = "Excel Files|*.xls;*.xlsx";
                    openFileDialog.Title = "Select an Excel File";

                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        string selectedFilePath = openFileDialog.FileName;
                                            
                        WorkBook workBook = WorkBook.Load(selectedFilePath);
                        WorkSheet workSheet = workBook.WorkSheets.First();

                        this.ACclassification = new ACclassification();

                        // Iterate over all cells in the worsheet
                        foreach (var cell in workSheet)
                        {
                            if (cell.Text != "")
                            {
                                if (cell.AddressString.Contains("A"))
                                {
                                    //AQUESTS IFS ES PODRIEN TREURE I ARREANDO
                                    //HP airplane
                                    if (cell.Text != "HP")
                                    {
                                        //Classify it girl
                                        this.ACclassification.AddModelClassification(cell.Text, "HP");
                                    }
                                }
                                else if (cell.AddressString.Contains("B"))
                                {
                                    //NR airplane
                                    if (cell.Text != "NR")
                                    {
                                        this.ACclassification.AddModelClassification(cell.Text, "NR");
                                    }
                                }
                                else if (cell.AddressString.Contains("C"))
                                {
                                    //NR+ airplane
                                    if (cell.Text != "NR+")
                                    {
                                        this.ACclassification.AddModelClassification(cell.Text, "NR+");
                                    }
                                }
                                else if (cell.AddressString.Contains("D"))
                                {
                                    //NR- airplane
                                    if (cell.Text != "NR-")
                                    {
                                        this.ACclassification.AddModelClassification(cell.Text, "NR-");
                                    }
                                }
                                else if (cell.AddressString.Contains("E"))
                                {
                                    //LP airplane
                                    if (cell.Text != "LP")
                                    {
                                        this.ACclassification.AddModelClassification(cell.Text, "LP");
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("Cell {0} has value '{1}'", cell.AddressString, cell.Text);
                                }

                            }                 

                        }
                        popUpLabel("✅ Correctly loaded " + Convert.ToString(this.ACclassification.classificationDictionary.Keys.Count) + " aircrafts classifications.");

                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    popUpLabel("❌ Something went wrong... Is the EXCEL file open somewhere else?");
                }
            }
        }

        private void importSID06RToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                try
                {
                    openFileDialog.Filter = "Excel Files|*.xls;*.xlsx";
                    openFileDialog.Title = "Select an Excel File";

                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        string selectedFilePath = openFileDialog.FileName;

                        WorkBook workBook = WorkBook.Load(selectedFilePath);
                        WorkSheet workSheet = workBook.WorkSheets.First();

                        // Iterate over all cells in the worsheet
                        foreach (var cell in workSheet)
                        {
                            if (cell.Text != "")
                            {
                                if (cell.AddressString.Contains("A"))
                                {
                                    if (cell.Text != "Misma_SID_G1")
                                    {
                                        this.ACclassification.SIDinfoDictionary06R.Add(cell.Text, "Misma_SID_G1");
                                    }
                                }
                                else if (cell.AddressString.Contains("B"))
                                {
                                    if (cell.Text != "Misma_SID_G2")
                                    {
                                        this.ACclassification.SIDinfoDictionary06R.Add(cell.Text, "Misma_SID_G2");
                                    }
                                }
                                else if (cell.AddressString.Contains("C"))
                                {
                                    if (cell.Text != "Misma_SID_G3")
                                    {
                                        this.ACclassification.SIDinfoDictionary06R.Add(cell.Text, "Misma_SID_G3");
                                    }
                                }                                
                                else
                                {
                                    Console.WriteLine("Cell {0} has value '{1}'", cell.AddressString, cell.Text);
                                }

                            }
                        }

                        popUpLabel("✅ Correctly loaded " + Convert.ToString(this.ACclassification.SIDinfoDictionary06R.Keys.Count) + " SIDs 06R.");

                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    popUpLabel("❌ Something went wrong... Is the EXCEL file open somewhere else?");
                }
            }
        }

        private void importSID24LToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                try
                {
                    openFileDialog.Filter = "Excel Files|*.xls;*.xlsx";
                    openFileDialog.Title = "Select an Excel File";

                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        string selectedFilePath = openFileDialog.FileName;

                        WorkBook workBook = WorkBook.Load(selectedFilePath);
                        WorkSheet workSheet = workBook.WorkSheets.First();

                        // Iterate over all cells in the worsheet
                        foreach (var cell in workSheet)
                        {
                            if (cell.Text != "")
                            {
                                if (cell.AddressString.Contains("A"))
                                {
                                    if (cell.Text != "Misma_SID_G1")
                                    {
                                        this.ACclassification.SIDinfoDictionary24L.Add(cell.Text, "Misma_SID_G1");
                                    }
                                }
                                else if (cell.AddressString.Contains("B"))
                                {
                                    if (cell.Text != "Misma_SID_G2")
                                    {
                                        this.ACclassification.SIDinfoDictionary24L.Add(cell.Text, "Misma_SID_G2");
                                    }
                                }
                                else if (cell.AddressString.Contains("C"))
                                {
                                    if (cell.Text != "Misma_SID_G3")
                                    {
                                        this.ACclassification.SIDinfoDictionary24L.Add(cell.Text, "Misma_SID_G3");
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("Cell {0} has value '{1}'", cell.AddressString, cell.Text);
                                }
                            }
                        }

                        popUpLabel("✅ Correctly loaded " + Convert.ToString(this.ACclassification.SIDinfoDictionary24L.Keys.Count) + " SIDs 24L.");

                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    popUpLabel("❌ Something went wrong... Is the EXCEL file open somewhere else?");
                }
            }
        }

        private void computeCompatibilitiesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Compute if the take offs are compatible here
            int i = 0;
            while(i < despeguesList.Count - 1)
            {
                //1. computeDistanceBetweenNextAC(despeguesList[i], despeguesList[i + 1])
                double realSeparation = 20; //NM

                //2. passem la distancia i mirem si compleix la minima per radar
                computeRadarCompatibility(realSeparation);

                //3. passem la distancia i mirem si es compatible per les esteles
                computeEstelaCompatibility(despeguesList[i], despeguesList[i + 1], realSeparation);

                //4. passem la distancia i mirem si es compleix per la LoA
                computeLoACompatibility(despeguesList[i], despeguesList[i + 1], realSeparation);

                i++;
            }
        }

        private void computeRadarCompatibility (double realSeparation)
        {
            //Separacion minima radar = 3NM
            if (realSeparation >= 3.0)
            {
                //Compatible segons la minima del radar
            }
            else
            {
                //No compatible segons el radar
            }


            //ANOTHER WAY TO MAKE STATISTICS LATER:
            double difference = realSeparation - 3.0;
            if (difference >= 0)
            {
                //The separation is bigger
            }
            else
            {
                //The separation is not bigger than 3NM and the difference is abs(difference)
            }
        }

        private void computeEstelaCompatibility (Ruta first, Ruta second, double realSeparation)
        {
            //Despeguen pel mateix lloc?
            //S'ha de mirar akgo mes de la ruta?!?!??!?!
            if (first.PistaDesp == second.PistaDesp)
            {
                double minSeparation = 0.0; //Separacion minima
                string sucesiva = second.Estela;
                switch (first.Estela)
                {
                    case "Super Pesada":
                        if (sucesiva == "Pesada") { minSeparation = 6.0; }
                        else if (sucesiva == "Media") { minSeparation = 7.0; }
                        else { minSeparation = 8.0; }
                        break;

                    case "Pesada":
                        if (sucesiva == "Pesada") { minSeparation = 4.0; }
                        else if (sucesiva == "Media") { minSeparation = 5.0; }
                        else { minSeparation = 6.0; }
                        break;

                    case "Mediana":
                        if (sucesiva == "Ligera") { minSeparation = 5.0; }
                        break;
                }
                Console.WriteLine("Minimum separation due Estela:");
                Console.WriteLine(minSeparation);

                if (realSeparation >= minSeparation)
                {
                    //Todo gucci, esteles compatibles
                }
                else
                {
                    //Esteles no compatibles
                }
            }
        }

        private void computeLoACompatibility(Ruta first, Ruta second, double realDistance)
        {



        }
    }
}

