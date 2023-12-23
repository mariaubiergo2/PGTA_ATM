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
using CsvHelper;
using System.Globalization;

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

        //Diccionari per fer estadistiques
        //TOTS ELS IDs i el seu ACpair
        Dictionary<string, List<AC_pair>> pairsDictionary;

        List<AC_pair> pairsList;

        //string1 identificador, string2 estela+loa+radar if ho incumpleix tot
        Dictionary<string, string> incumplimientos;


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

                    if (ac.I240_TId != "")
                    {
                        //If the dictionary already has the key
                        if (this.aircraftsInfoDic.ContainsKey(ac.I240_TId))
                        {
                            this.aircraftsInfoDic[ac.I240_TId].Add(ac);
                        }
                        else
                        {
                            List<ACinfo> list = new List<ACinfo>();
                            list.Add(ac);
                            this.aircraftsInfoDic.Add(ac.I240_TId, list);
                        }
                    }
                    else
                    {
                        Console.WriteLine(ac.I161_Tn + " has no ID");
                    }
                                       
                }

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
            this.ACclassification = new ACclassification();

            try
            {
                WorkBook workBook = WorkBook.Load(rutaArchivo);
                WorkSheet workSheet = workBook.WorkSheets.First();

                progressLbl.Visible = true;
                progressBar1.Visible = true;
                
                // Iterate over all cells in the worsheet
                int row = 2;
                while (row <= workSheet.RowCount)
                {
                    int valProgress = Convert.ToInt32(Convert.ToDouble(row) / workSheet.RowCount * 100);
                    ReportProgess(valProgress);

                    Ruta despegue = new Ruta(
                        Convert.ToString(workSheet["A" + Convert.ToString(row)]),
                        Convert.ToString(workSheet["B" + Convert.ToString(row)]),
                        Convert.ToString(workSheet["C" + Convert.ToString(row)]),
                        Convert.ToString(workSheet["D" + Convert.ToString(row)]),
                        Convert.ToString(workSheet["E" + Convert.ToString(row)]),
                        Convert.ToString(workSheet["F" + Convert.ToString(row)]),
                        Convert.ToString(workSheet["G" + Convert.ToString(row)]),
                        Convert.ToString(workSheet["H" + Convert.ToString(row)]),
                        Convert.ToString(workSheet["I" + Convert.ToString(row)])
                        );

                    //No s'afegeixen les que no surten per la 24L o 06R
                    if (despegue.id != null)
                    {                        
                        this.despeguesList.Add(despegue);
                        this.ACclassification.estelaDictionary.Add(despegue.id, despegue.Estela);
                    }

                    row++;
                }

                progressLbl.Visible = false;
                progressBar1.Visible = false;


                popUpLabel("✅ Correctly loaded " + Convert.ToString(despeguesList.Count)+ " take off!");
                                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                popUpLabel("❌ Something went wrong... Is the EXCEL file opened somewhere else?");
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

                        //this.ACclassification = new ACclassification();

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
                                //string res = usefulFunctions.RemoveEnd(cell.Text, 2);
                                string res = cell.Text;

                                if (cell.AddressString.Contains("A"))
                                {
                                    if (cell.Text != "Misma_SID_G1")
                                    {
                                        this.ACclassification.SIDDictionary.Add(res, "G1_06R");
                                    }
                                }
                                else if (cell.AddressString.Contains("B"))
                                {
                                    if (cell.Text != "Misma_SID_G2")
                                    {
                                        this.ACclassification.SIDDictionary.Add(res, "G2_06R");
                                    }
                                }
                                else if (cell.AddressString.Contains("C"))
                                {
                                    if (cell.Text != "Misma_SID_G3")
                                    {
                                        this.ACclassification.SIDDictionary.Add(res, "G3_06R");
                                    }
                                }                                
                                else
                                {
                                    Console.WriteLine("Cell {0} has value '{1}'", cell.AddressString, cell.Text);
                                }

                            }
                        }

                        popUpLabel("✅ Correctly loaded " + Convert.ToString(this.ACclassification.SIDDictionary.Keys.Count) + " total SIDs.");

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
                                //string res = usefulFunctions.RemoveEnd(cell.Text, 2);
                                string res = cell.Text;

                                if (cell.AddressString.Contains("A"))
                                {
                                    if (cell.Text != "Misma_SID_G1")
                                    {
                                        this.ACclassification.SIDDictionary.Add(res, "G1_24L");
                                    }
                                }
                                else if (cell.AddressString.Contains("B"))
                                {
                                    if (cell.Text != "Misma_SID_G2")
                                    {
                                        this.ACclassification.SIDDictionary.Add(res, "G2_24L");
                                    }
                                }
                                else if (cell.AddressString.Contains("C"))
                                {
                                    if (cell.Text != "Misma_SID_G3")
                                    {
                                        this.ACclassification.SIDDictionary.Add(res, "G3_24L");
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("Cell {0} has value '{1}'", cell.AddressString, cell.Text);
                                }
                            }
                        }

                        popUpLabel("✅ Correctly loaded " + Convert.ToString(this.ACclassification.SIDDictionary.Keys.Count) + " total SIDs.");

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
            this.pairsDictionary = new Dictionary<string, List<AC_pair>>();
            this.pairsList = new List<AC_pair>();
            this.incumplimientos = new Dictionary<string, string>();

            //Compute if the take offs are compatible here
            int i = 0;

            while(i < despeguesList.Count - 1)
            {
                //1. computeDistanceBetweenNextAC(despeguesList[i], despeguesList[i + 1])
                Ruta despegue1 = despeguesList[i];
                Ruta despegue2 = despeguesList[i + 1];

                //1.1 Mirem a quina hora surt el segon despegue
                DateTime startHour = despegue2.HoraDespegue;

                //1.2 Busquem els ACinfo a partir d'aquella hora amb un marge de +-3s?
                List<ACinfo> info1 = aircraftsInfoDic[despegue1.indicativo];
                List<ACinfo> info2 = aircraftsInfoDic[despegue2.indicativo];

                //Hi ha algun ACinfo que compleixi aixòs?
                bool found1 = false;
                int initialACinfoPosition1 = -1;
                while (initialACinfoPosition1 < info1.Count && !found1)
                {
                    initialACinfoPosition1++;

                    if (initialACinfoPosition1 != info1.Count)
                    {
                        found1 = usefulFunctions.isDateBetween((info1[initialACinfoPosition1].I140_ToD), (info1[initialACinfoPosition1].I140_ToD).AddSeconds(3), startHour);
                    }
                }

                if (found1)
                {
                    //Busquem on comença el despegue2 en el seu llistat d'asteriks 
                    bool found2 = false;
                    int initialACinfoPosition2 = -1;
                    while (initialACinfoPosition2 < info2.Count && !found2)
                    {
                        initialACinfoPosition2++;

                        if (initialACinfoPosition2 != info2.Count)
                        {
                            found2 = usefulFunctions.isDateBetween((info2[initialACinfoPosition2].I140_ToD), (info2[initialACinfoPosition2].I140_ToD).AddSeconds(3), startHour);
                        }
                    }
                    if (found2)
                    {
                        //ARA val la pena comparar
                        ACinfo startingPoint1;
                        ACinfo startingPoint2;                        

                        int len1 = info1.Count;
                        int len2 = info2.Count;

                        bool worthit = true;

                        while (initialACinfoPosition1 < len1 && initialACinfoPosition2 < len2 && worthit)
                        {
                            //Comparem:
                            startingPoint1 = info1[initialACinfoPosition1]; //Busquem partint des de llistat1
                            startingPoint2 = info2[initialACinfoPosition2];

                            //saber si entre startingPoint1 i startingPoint2 hi ha una diferencia de +-2s
                            bool inInterval = usefulFunctions.differenceTimesInInterval(startingPoint1.I140_ToD, startingPoint2.I140_ToD, 2);
                            
                            if (inInterval)
                            {
                                AC_pair pair = this.ACclassification.SetACpairs(despegue1, despegue2, startingPoint1, startingPoint2);
                                                               
                                //Generem un diccionari per fer estadístiques 
                                if (this.pairsDictionary.ContainsKey(despegue2.indicativo))
                                {
                                    this.pairsDictionary[despegue2.indicativo].Add(pair);
                                }
                                else
                                {
                                    //Diccionari amb els incumpliments afegim la parella
                                    List<AC_pair> n = new List<AC_pair>();
                                    n.Add(pair);
                                    this.pairsDictionary.Add(despegue2.indicativo, n);                                    
                                }

                                //Llistat per fer el CSV
                                this.pairsList.Add(pair);

                                //LLAVORS PASSAR AL SEGUENT
                                initialACinfoPosition1++;
                                initialACinfoPosition2++;

                            }
                            else
                            {
                                initialACinfoPosition2++;
                            }

                        }                      
                        
                    }
                    else
                    {
                        //Emmagatzemar que no s'ha trobat cap Asteriks per aquell despegue
                    }
                }                

                i++;
            }

            popUpLabel("✅ Correctly done the compatibilities test!");
        }



        private void gMapControl1_Load(object sender, EventArgs e)
        {

        }

        private void saveCompatibilitiesInCSVToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();

            // Set initial directory and file name filters if needed
            saveFileDialog.InitialDirectory = this.projectDirectory;
            saveFileDialog.Filter = "CSV files (*.csv)|*.csv";

            try
            {
                // Show the dialog and check if the user clicked OK
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    // Get the selected file name
                    string filePath = saveFileDialog.FileName;

                    // Your existing code to write to CSV
                    using (var writer = new StreamWriter(filePath))
                    using (var csvWriter = new CsvWriter(writer, CultureInfo.InvariantCulture))
                    {
                        csvWriter.WriteRecords<AC_pair>(pairsList);
                    }

                    if (pairsList.Count != 0)
                    {
                        popUpLabel("✅ Your file is correctly saved!");
                    }
                    else
                    {
                        popUpLabel("✅ Your file is correctly saved! Yet is empty.");
                    }

                }
            }
            catch
            {
                popUpLabel("❌ Something went wrong... Please, try again!");
            }


        }

        private void tRIALSToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string d1 = "05/02/2023 14:10:20";
            string d2 = "05/02/2023 14:10:19";
            
            Console.WriteLine(d1.Substring(1));
            Console.WriteLine(Convert.ToString(d1.ToCharArray()[0]));

        }

        private void getCompleteTakeOffsCSVToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //RETURN A CSV FILE WITH ALL THE DEPARTURES BUT WITH AMPLIFIED INFORMATION :)

            int i = 0;
            while (i < despeguesList.Count)
            {
                //Ja tenim els conflictes
                //Ja tenim la minima distancia
                //Ara volem la classificació
                despeguesList[i].setClassification(this.ACclassification.GetACclassification(despeguesList[i].TipoAeronave));
                //I ara la sid!
                despeguesList[i].setGroupSID(this.ACclassification.GetSIDGroup(despeguesList[i].ProcDesp));

                if (pairsDictionary.ContainsKey(despeguesList[i].indicativo))
                {
                    foreach (AC_pair pair in this.pairsDictionary[despeguesList[i].indicativo])
                    {
                        this.despeguesList[i].setIncumplimentos(pair);
                        this.despeguesList[i].setMinimaDistancia(pair.real_dist);
                    }

                    string res = "";
                    if (despeguesList[i].incumpleEstelaDespegue)
                    {
                        res = res + "Estela";
                    }
                    if (despeguesList[i].incumpleLoADespegue)
                    {
                        res = res + "+LoA";
                    }
                    if (despeguesList[i].incumpleRadarDespegue)
                    {
                        res = res + "+Radar";
                    }
                    if (res == "")
                    {
                        despeguesList[i].incumplimientosList = "Non";
                    }
                    else
                    {
                        string first = Convert.ToString(res.ToCharArray()[0]);
                        if (first == "+")
                        {
                            despeguesList[i].incumplimientosList = res.Substring(1);
                        }
                        else
                        {
                            despeguesList[i].incumplimientosList = res;
                        }

                    }

                }


                i++;
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog();

            // Set initial directory and file name filters if needed
            saveFileDialog.InitialDirectory = this.projectDirectory;
            saveFileDialog.Filter = "CSV files (*.csv)|*.csv";

            try
            {
                // Show the dialog and check if the user clicked OK
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    // Get the selected file name
                    string filePath = saveFileDialog.FileName;

                    // Your existing code to write to CSV
                    using (var writer = new StreamWriter(filePath))
                    using (var csvWriter = new CsvWriter(writer, CultureInfo.InvariantCulture))
                    {
                        csvWriter.WriteRecords<Ruta>(despeguesList);
                    }

                    if (despeguesList.Count != 0)
                    {
                        popUpLabel("✅ Your file is correctly saved!");
                    }
                    else
                    {
                        popUpLabel("✅ Your file is correctly saved! Yet is empty.");
                    }

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                popUpLabel("❌ Something went wrong... Please, try again!");
            }

        }
    }
}

