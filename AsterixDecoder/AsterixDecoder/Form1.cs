using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using CsvHelper;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using System.Xml;
using GMap.NET.WindowsForms.ToolTips;

namespace AsterixDecoder
{
    public partial class MainWindow : Form
    {
        //Decoding
        //-----------------------------------------------------------

        //Octet position analysed/decoded
        int n_datarecord;
        //All the CAT048 data units decoded
        List<CAT048> elements = new List<CAT048>();
        //Number of csv files that have been done
        int n_outputCSV;

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

        public MainWindow()
        {
            InitializeComponent();

            viewPanel.Visible = false;
            csvGridView.Visible = false;
            progressBar1.Visible = false;
            progressLbl.Visible = false;
            gmap.Visible = false;

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

            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;


            //CSV grid 
            generateCSVgrid();
            n_outputCSV = 0;

            infoLbl.Visible = false;
            firstTimeSimOpened = true;

        }


        //Customize the CSV grid
        private void generateCSVgrid()
        {
            csvGridView.DefaultCellStyle.Font = new Font("Cascadia Code", 11);
            csvGridView.CellBorderStyle = DataGridViewCellBorderStyle.Single;
            csvGridView.GridColor = SystemColors.ActiveBorder;
            csvGridView.MultiSelect = false;
            csvGridView.ReadOnly = true;
            csvGridView.AllowUserToAddRows = false;
            csvGridView.AllowUserToDeleteRows = false;
            csvGridView.AllowUserToOrderColumns = true;
            csvGridView.AllowUserToResizeColumns = true;
            csvGridView.AllowUserToResizeRows = true;
            csvGridView.AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = System.Drawing.Color.White
            };
            csvGridView.BackgroundColor = System.Drawing.Color.White;
            csvGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
            csvGridView.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                Font = new System.Drawing.Font("Arial", 12, System.Drawing.FontStyle.Bold),
                BackColor = System.Drawing.Color.SteelBlue,
                ForeColor = System.Drawing.Color.White
            };
            csvGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        }

        //Decoding to CSV
        private void decodeAll(string path, int init, int final)
        {
            //Initiate progress bar
            progressBar1.Value = 0;
            progressLbl.Text = "Loading...";
            progressLbl.Visible = true;

            //Decodify
            decodify(path, init, final);
        }

        //Decoding to CSV
        private void ReportProgess(int percent)
        {
            progressLbl.Visible = true;
            progressBar1.Visible = true;
            progressLbl.Text = "Loading... " + Convert.ToString(percent) + " %";
            progressLbl.Refresh();
            progressBar1.Value = percent;

            if (percent == 100)
            {
                progressLbl.Visible = false;
                progressBar1.Visible = false;
            }
        }

        //Decoding to CSV
        public void decodify(string path, int init, int final)
        {
            byte[] message = File.ReadAllBytes(path);
            int i = 0;

            // Number of total data records in the file
            this.n_datarecord = 0;

            while (i < message.Length)
            {
                //First octet describes the category of the message
                byte cat = message[i];

                //Second and third byte equivals to the length of the data record 
                byte b1 = message[i + 1];
                byte b2 = message[i + 2];

                //Getting the length of the message 
                int length_dr = b1 << 8 | b2;

                if (cat == 48)
                {
                    List<byte> datablock = new List<byte>();
                    List<byte> FSPEC = new List<byte>();

                    //Identification of the length of each FSPEC. 
                    int j = 1;
                    byte octet = message[i + 3];
                    if (octet % 2 != 0) // This octet is odd (ends in 1) then the next one is included
                    {
                        // j = 1;
                        FSPEC.Add(octet);
                        while (octet % 2 != 0) // The last octet ends in 0 indicating there are no more
                        {
                            octet = message[i + 3 + j];
                            FSPEC.Add(octet);
                            j++;
                        }
                    }
                    else
                    {
                        // j = 1;
                        FSPEC.Add(octet);
                    }

                    //Length of the Asterix record length minus the Cat octet and the 2 length octets 
                    while (j < (length_dr - 3))
                    {
                        datablock.Add(message[i + j + 3]);
                        j++;
                    }

                    //Funció per a decodificar la cat48 
                    CAT048 element = new CAT048(FSPEC, datablock, 0, n_datarecord);

                    if (element.TYP == "No detection" || element.TYP == "Single PSR detection" || element.TYP == "Single SSR detection" || element.TYP == "SSR+PSR detection" || element.Mode_3A == "7777")
                    { }
                    else
                    {
                        this.elements.Add(element);

                    }
                }

                else
                {
                    //Passem olimpicament del missatge
                }

                int valProgress = Convert.ToInt32(Convert.ToDouble(i) / message.Length * 100);
                ReportProgess(valProgress);

                this.n_datarecord++;
                i = i + length_dr;

            }

        }

        //Reading a CSV file and showing in the DataGridView
        public DataTable readCSV(string filePath)
        {
            DataTable dt = new DataTable();
            try
            {
                string[] lines = System.IO.File.ReadAllLines(filePath);
                if (lines.Length > 0)
                {
                    //first line to create header
                    string firstLine = lines[0];
                    string[] headerLabels = firstLine.Split(',');
                    foreach (string headerWord in headerLabels)
                    {
                        dt.Columns.Add(new DataColumn(headerWord));
                    }
                    //For Data
                    for (int i = 1; i < lines.Length; i++)
                    {
                        string pattern = @"(?<=,|^)(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";
                        string[] dataWords = Regex.Split(lines[i], pattern);
                        DataRow dr = dt.NewRow();
                        int columnIndex = 0;

                        foreach (string headerWord in headerLabels)
                        {
                            string cleanedItem = dataWords[columnIndex + 1].Trim('"').Trim(',').Replace('"', ' ');
                            dr[headerWord] = cleanedItem;
                            columnIndex++;
                        }
                        dt.Rows.Add(dr);
                    }
                }
                if (dt.Rows.Count > 0)
                {
                    csvGridView.DataSource = dt;
                }
                return dt;
            }
            catch
            {
                popUpLabel("❌ Something went wrong... Please, try again!");
                return dt;
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
            if (elements.Count != 0)
            {
                csvGridView.Visible = true;
            }
        }

        //Decoding to CSV
        private void importBinaryFileToDecodeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Hide simulation stuff
            viewHideSimulation(false);

            try
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Filter = "AST File (*.ast)|*.ast|BIN Files (*.bin)|*.bin";
                using (ofd)
                {
                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        if (elements.Count != 0)
                        {
                            this.elements = new List<CAT048>();
                            this.aircraftsList = new Dictionary<string, List<CAT048_simulation>>();
                            this.aircraftsMarkers = new Dictionary<string, RotatableMarker>();
                            gmap.Overlays.Remove(markers);
                            this.markers = new GMapOverlay("markers");
                            showTime();
                        }


                        string path = ofd.FileName.ToString();

                        decodeAll(path, 0, 0);

                        popUpLabel("⏳ Preparing the decoded data items...");

                        //Visualize CSV in the grid
                        csvGridView.DataSource = elements;
                        csvGridView.Visible = true;
                    }
                }

                this.isSimulating = true;
            }
            catch
            {
                popUpLabel("❌ Something went wrong... Please, try again!");
            }
            
        }


        //Save to CSV
        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();

            // Set initial directory and file name filters if needed
            saveFileDialog.InitialDirectory = this.projectDirectory;
            saveFileDialog.Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*";

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
                        csvWriter.WriteRecords<CAT048>(elements);
                    }

                    if (elements.Count != 0)
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

        public void viewHideSimulation(bool visible)
        {
            viewPanel.Visible = visible;
            gmap.Visible = visible;
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
            csvGridView.Visible = visible;
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
                gmap.MapProvider = GMapProviders.GoogleMap;
                gmap.Position = new PointLatLng(41.298, 2.080);
                gmap.MinZoom = 2;
                gmap.MaxZoom = 24;
                gmap.Zoom = 9;
                gmap.AutoScroll = true;
                gmap.DragButton = MouseButtons.Left;
                gmap.CanDragMap = true;

                IsMapas.SelectedItem = "Google Maps Callejero";

                Controls.Add(gmap);

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

            gmap.Visible = true;
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

                    gmap.Visible = true;

                    //Generate the dictionary with the AC_ID and its associated marker
                    aircraftsMarkers = new Dictionary<string, RotatableMarker>();

                    foreach (string id in aircraftsList.Keys)
                    {
                        plotTheCurrentDataItemForAGivenTime(id);
                    }

                    //First you generate the markers and then you put it in the map
                    gmap.Overlays.Add(markers);

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
                gmap.MapProvider = GMapProviders.GoogleSatelliteMap;
            if (IsMapas.Text == "Google Maps Callejero")
                gmap.MapProvider = GMapProviders.GoogleMap;
            if (IsMapas.Text == "Google Maps Híbrido")
                gmap.MapProvider = GMapProviders.GoogleHybridMap;
            if (IsMapas.Text == "OpenStreetMap")
                gmap.MapProvider = GMapProviders.OpenStreetMap;
            if (IsMapas.Text == "OpenCycleMap")
                gmap.MapProvider = GMapProviders.OpenCycleMap;

            gmap.Refresh();

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

            gmap.Refresh();
            
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

            gmap.Refresh();

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

                        DataTable dt = readCSV(path);

                        //Visualize CSV in the grid
                        csvGridView.DataSource = dt;
                        csvGridView.Visible = true;

                        popUpLabel("⏳ Preparing the decoded data items...");
                    }
                }

                this.isSimulating = true;
            }
            catch
            {
                popUpLabel("❌ Something went wrong... Please, try again!");
            }

        }
    }
}

