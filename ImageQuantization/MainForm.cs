using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace ImageQuantization
{
   
    public partial class MainForm : Form
    {
        /*
        Finding Distinct Colors Function
        Summary : Calculate the number of distinct colors in an image
        Parameters : Input image as 2D array of RGPPixel
        Return : List of distinct colors each distinct color of type RGBPixelD             
        */    
        public List<RGBPixelD> findingDistinctColors (RGBPixel [,] colorsarr)//O(N^2)
        {
            int height = ImageOperations.GetHeight(colorsarr); //O(1)
            int width = ImageOperations.GetWidth(colorsarr);//O(1)
            List<RGBPixelD> newDistinct = new List<RGBPixelD>();//O(1)
            Dictionary<double, int> checker = new Dictionary<double, int>(); //O(1)
            RGBPixelD temp; //O(1)
            for (int i = 0; i< height; i++) //O(N^2)
            {
                for (int j = 0; j < width; j++)//O(N)
                {
                    temp.blue = colorsarr[i, j].blue;//O(1)
                    temp.green = colorsarr[i, j].green;//O(1)
                    temp.red = colorsarr[i, j].red;//O(1)
                    double key = 0.5 * (temp.red + temp.blue) * (temp.red + temp.blue + 1) + temp.blue; //O(1)
                    key = 0.5 * (temp.green + key) * (temp.green + key + 1) + key; //O(1)
                    if (!checker.ContainsKey(key)) //O(1)
                    {
                        checker.Add(key, 1); //O(1)
                        newDistinct.Add(temp);//O(1)
                    }
                  
                }
            }
            return newDistinct ;//O(1)
        }

       /*
        Euclidean Equation Function
        Summary : Calculates distance between two pixels
        Parameters : Two pixels as RGBPixelD
        Return : Distance between two pixels as double
        */
        public static double euclideanEquation (RGBPixelD pix1 , RGBPixelD pix2)               //O(1)
        {
            return  Math.Sqrt( (pix1.red-pix2.red)* (pix1.red - pix2.red) + ((pix1.green - pix2.green)* (pix1.green - pix2.green)) + 
                ( (pix1.blue - pix2.blue)* (pix1.blue - pix2.blue) ) ) ;      //O(1)
        }

        /* Get Minimum Function
         * Summary : Finding unvisited node with miminum distance to reach in a graph
         * Parameters : distances array carrying minimum distance to reach node as double, visited array 
         *              indicating whether this node is visited before as boolen array
         */
        public static int getMinimum(double [] dis, bool [] vis)   //O(V)
        {   double minimum = double.MaxValue;                      //O(1)
            int index = -1;                                       //O(1)
            int distCount = dis.Length;                           //O(1)
            for (int i = 0; i< distCount; i++)                  //O(V)
            {
                if (dis[i] < minimum && !vis[i]) //O(1)
                {
                    minimum = dis[i];                        //O(1)
                    index = i;                                //O(1)
            }
                }
            return index; //O(1)

        }

        //Public Variables for all functions
        public static double[] distances; //O(1)
        public static bool[] visited; //O(1)
        public static int[] parent; //O(1)
        public static int[] clustersIndices; //O(1)
        public static Dictionary<int, List<int>> map; //O(1)


        /* Minimum Spanning Tree Function
        * Summary : Construct the minimum spanning tree from an undirected weighted graph
        * Parameters : Distinct colors as a list of RGPPixelD
        * Return : Summation of distances in MST as double  
         */
        public static double MST(List<RGBPixelD> distinctColors)    //O(E)
        {
            double summation = 0;
            parent = new int[distinctColors.Count];       //O(1)
            distances = new double[distinctColors.Count];     //O(1)         
            visited = new bool[distinctColors.Count];          //O(1)
            int distCount = distinctColors.Count;                     //O(1)
            for (int i = 0; i < distCount; i++)                       //O(v)
            {
                distances[i] = double.MaxValue;                      //O(1)
            }

            distances[0] = 0;                                        //O(1)
            parent[0] = -1;                                          //O(1)

            for (int i = 0; i < distCount - 1;i++)  //O(E)=O(v-1)*O(v)
            {
                int index = getMinimum(distances, visited);         //O(v)
                visited[index] = true;                              //O(1)

                for (int j = 0; j< distCount; j++)                  //O(v)
                {
                    if(euclideanEquation(distinctColors[index],distinctColors[j])!=0 && !visited[j] && euclideanEquation(distinctColors[index], distinctColors[j]) < distances[j])//O(1)
                    {
                        parent[j] = index;                          //O(1)
                        distances[j] = euclideanEquation(distinctColors[index], distinctColors[j]);     //O(1)
                    }
                }
            }
            for (int i = 0; i < distances.Length; i++)              //O(v)
            {
                summation += distances[i];                          //O(1)
            }
            return summation;
        }

            
        /* Cut Minimum Spaning tree function
         * Summary : Cuts minimum spanning tree according to number of clusters and group each cluster together in a hashset
         * Parameters : Number of clusters as integer
         * Return : Clusters formed as a list of hashsets each representing a single cluster
         */
        public List<HashSet<int>> cutMST(int noClusters) 
        {
            int distinctColors = distances.Length; //O(1)
            clustersIndices = new int[noClusters]; // O(1)
            clustersIndices[0] = 0; //O(1)
            for (int i = 1; i < noClusters; i++) //O(K*D)
            {
                double maxDistance = distances[0]; //O(1)
                int indexOfMaxDistance = 0; //O(1)
                for (int j = 0; j < distinctColors; j++) //O(D)
                {
                    if (distances[j] > maxDistance) //O(1)
                    {
                        maxDistance = distances[j]; //O(1)
                        indexOfMaxDistance = j; //O(1)
                    }
                }
                parent[indexOfMaxDistance] = -1; //O(1)
                distances[indexOfMaxDistance] = 0; //O(1)
                clustersIndices[i] = indexOfMaxDistance; //O(1)
            }

            map = new Dictionary<int, List<int>>(); //O(1)
            for (int i = 0; i < distinctColors; i++) //O(D)
            {
                int temp = parent[i];  //O(1)
                if (temp == -1)  //O(1)
                    continue;   //O(1)
                if (map.ContainsKey(temp)) //O(1)
                {
                    map[temp].Add(i); //O(1)
                } 
                else
                {
                    List<int> tempList = new List<int>(); //O(1)
                    tempList.Add(i); //O(1)
                    map.Add(temp, tempList); //O(1)
                }            }
            List<HashSet<int>> clusters = new List<HashSet<int>>(noClusters); //O(1)
            for(int i = 0; i< noClusters;i++) //O(K)
            {
                int clusterInitial = clustersIndices[i]; //O(1)
                HashSet<int> cluster = new HashSet<int>(); //O(1)
                Queue<int> temp = new Queue<int>(); //O(1)
                temp.Enqueue(clusterInitial); //O(1)
                int tempVal = 0;//O(1)
                while (true) // Max O(D)
                {
                    if (temp.Count == 0) //O(1)
                    {
                        clusters.Add(cluster); //O(1)
                        break;//O(1)
                    }
                    tempVal = temp.Dequeue();//O(1)
                    cluster.Add(tempVal);//O(1)
                    if (map.ContainsKey(tempVal))//O(1)
                    { List<int> children = map[tempVal];//O(1)
                        int c = children.Count;//O(1)
                        for (int k = 0; k < c; k++) // O(Childrens of the current parent ) Max O(D) 
                            temp.Enqueue(children[k]);//O(1)
                    }
                }
               
            }
            return clusters;//O(1)
        }
        //***************************************************************************
        /* Calculates Average of clusters function
         * Summary : Calculates average of each cluster and maps the cluster colors to this average
         * Parameters : Clusters as a list of hashsets, distinct colors as a list of RGPixelD
         * Return : Map of color as a key represented as a double and it's corresponding color average as a RGBPixel
         */
        public Dictionary<double, RGBPixel> calculateAverageOfClusters(List<HashSet<int>> clusters, List<RGBPixelD> distinctColors) //O(D)
        {   int size = clusters.Count;//O(1)
            Dictionary<double, RGBPixel> pallete = new Dictionary<double, RGBPixel>();//O(1)
            for (int i = 0; i < size; i++) // MAX O(D) as the total comlexity bounded by the number of clusters * the size of each one added together 
            {
                int clusterSize = clusters[i].Count; //O(1)
                double redSum = 0; //O(1)
                double blueSum = 0; //O(1)
                double greenSum = 0; //O(1)
                List<int> current = new List<int>(clusters[i]);//O(1)
                for (int j =  0; j < clusterSize;j++) //O(# of ditinct colors in this cluster) MAX O(D)
                {
                    int index = current[j];//O(1)
                    redSum += distinctColors[index].red;//O(1)
                    blueSum += distinctColors[index].blue;//O(1)
                    greenSum += distinctColors[index].green;//O(1)
                }
                byte averageRed = (byte) (redSum/clusterSize);//O(1)
                byte averageBlue = (byte) (blueSum/clusterSize);//O(1)
                byte averageGreen = (byte) (greenSum/clusterSize);//O(1)
                RGBPixel temp = new RGBPixel();//O(1)
                temp.red = averageRed;//O(1)
                temp.blue = averageBlue;//O(1)
                temp.green = averageGreen;//O(1)

                for (int j = 0; j< clusterSize;j++) //O(# of ditinct colors in this cluster) MAX O(D)
                {
                    int index = current[j];//O(1)
                  //  RGBPixel rGB = new RGBPixel();//O(1)
                    double red = distinctColors[index].red;//O(1)
                    double blue =distinctColors[index].blue;//O(1)
                    double green = distinctColors[index].green;//O(1)
                    double key = 0.5 * (red + blue) * (red + blue + 1) + blue;
                     key = 0.5 * (green + key) * (green + key + 1) + key;
                    pallete.Add(key, temp);//O(1)
                }
            }
            return pallete;//O(1)
        }
        /*
         * Colouring Function
         * Summary : Quantizes the original image by replacing each pixel with it's corresponding average
         * Parameters : Orginal Image as 2D array of RGBPixel, Colour Pallete as a dictionary of color as key represented as double and it's corresponding average represented as a RGBPixel
         * Return : Quantized Image as 2D array of RGBPixel
         */
        public RGBPixel[,] colouring(RGBPixel[,] matrix, Dictionary<double, RGBPixel> pallete) //O(N^2)
        {
            
            int height = ImageOperations.GetHeight(matrix); //O(1)
            int width = ImageOperations.GetWidth(matrix);//O(1)          
            for (int i = 0; i< height ;i++) // O(H*W) = O(N^2)
            {
                for(int j = 0; j < width ;j++) // O(W)
                {
                    double red = matrix[i, j].red;//O(1)
                    double blue = matrix[i, j].blue;//O(1)
                    double green = matrix[i, j].green;//O(1)
                    double key = 0.5 * (red + blue) * (red + blue + 1) + blue; //O(1)
                    key = 0.5 * (green + key) * (green + key + 1) + key; //O(1)

                    matrix[i, j] = pallete[key]; //O(1)
                }
            }
            return matrix;//O(1)
        }
        public MainForm()
        {
            InitializeComponent();
        }

        RGBPixel[,] ImageMatrix;
        private void btnOpen_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                //Open the browsed image and display it
                string OpenedFilePath = openFileDialog1.FileName;
                ImageMatrix = ImageOperations.OpenImage(OpenedFilePath);
                ImageOperations.DisplayImage(ImageMatrix, pictureBox1);
            }
            txtWidth.Text = ImageOperations.GetWidth(ImageMatrix).ToString();
            txtHeight.Text = ImageOperations.GetHeight(ImageMatrix).ToString();
        }

        private void btnGaussSmooth_Click(object sender, EventArgs e)
        {
            Stopwatch timeTaken = new Stopwatch();
            timeTaken.Start();
            List<RGBPixelD> dis = findingDistinctColors(ImageMatrix);
            
            MSTSumTxt.Text = MST(dis).ToString();
            distColor.Text = dis.Count.ToString();
            colouring(ImageMatrix, calculateAverageOfClusters(cutMST(Convert.ToInt32( noClusters.Text)), dis));
            timeTaken.Stop();
            timeTakenText.Text = timeTaken.Elapsed.ToString();
            ImageOperations.DisplayImage(ImageMatrix, pictureBox2);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}


