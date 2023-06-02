using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Mahdyar_Library;
using Mahdyar_Library.Classes;

namespace Mahdyar_Library_Tester
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        AStar<Cls_National, Cls_Door, Cls_Drink, Cls_Smoking, Cls_Animal> aStar =
            new AStar<Cls_National, Cls_Door, Cls_Drink, Cls_Smoking, Cls_Animal>();

        AStar<Agent, Work, Date> aStar2 = new AStar<Agent, Work, Date>();

        public MainWindow()
        {
            InitializeComponent();

            //  var t2 = File.ReadAllText(@"d:\s5.txt").Ext_XmlDeserialize<smses>();
            // xmldata s =new xmldata();
            //  s = s.load(@"d:\s5.xml") as xmldata;


        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            List<Cls_National> Item1States = new List<Cls_National>();
            List<Cls_Door> Item2States = new List<Cls_Door>();
            List<Cls_Drink> Item3States = new List<Cls_Drink>();
            List<Cls_Smoking> Item4States = new List<Cls_Smoking>();
            List<Cls_Animal> Item5States = new List<Cls_Animal>();

            Item1States.Add(new Cls_National() { Nationality = "انگلیس" });
            Item1States.Add(new Cls_National() { Nationality = "سوئد" });
            Item1States.Add(new Cls_National() { Nationality = "دانمارک" });
            Item1States.Add(new Cls_National() { Nationality = "آلمان" });
            Item1States.Add(new Cls_National() { Nationality = "نروژ" });

            Item2States.Add(new Cls_Door() { HomeDoorColor = "زرد" });
            Item2States.Add(new Cls_Door() { HomeDoorColor = "سبز" });
            Item2States.Add(new Cls_Door() { HomeDoorColor = "آبی" });
            Item2States.Add(new Cls_Door() { HomeDoorColor = "قرمز" });
            Item2States.Add(new Cls_Door() { HomeDoorColor = "سفید" });

            Item3States.Add(new Cls_Drink() { Drink = "آب" });
            Item3States.Add(new Cls_Drink() { Drink = "آبجو" });
            Item3States.Add(new Cls_Drink() { Drink = "شیر" });
            Item3States.Add(new Cls_Drink() { Drink = "قهوه" });
            Item3States.Add(new Cls_Drink() { Drink = "چای" });

            Item4States.Add(new Cls_Smoking() { Smoking = "Pall Mall" });
            Item4States.Add(new Cls_Smoking() { Smoking = "Dunhill" });
            Item4States.Add(new Cls_Smoking() { Smoking = "Blends" });
            Item4States.Add(new Cls_Smoking() { Smoking = "Blue Master" });
            Item4States.Add(new Cls_Smoking() { Smoking = "Prince" });

            Item5States.Add(new Cls_Animal() { Animal = "پرنده" });
            Item5States.Add(new Cls_Animal() { Animal = "اسب" });
            Item5States.Add(new Cls_Animal() { Animal = "گربه" });
            Item5States.Add(new Cls_Animal() { Animal = "سگ" });
            Item5States.Add(new Cls_Animal() { Animal = "ماهی" });
            
            aStar.Init(Item1States, Item2States, Item3States,Item4States,Item5States);

            aStar.Evt_Promissed += AStar_Evt_Promissed;
            aStar.SearchTree();
            var bhgfdhgfdh = aStar.ResultsBranch.FirstOrDefault();

            //var text = richTextBox.Ext_GetText().Replace("\"", "'").Replace("-", "").Replace("\n", "");
            //richTextBox.Ext_SetText(text);
        }

        private int AStar_Evt_Promissed(AStar<Cls_National, Cls_Door, Cls_Drink, Cls_Smoking, Cls_Animal>.TreeNode arg)
        {
            if (arg.entity1.Nationality == "سوئد" && arg.entity5.Animal != "سگ") return 5;
            if (arg.entity1.Nationality == "انگلیس" && arg.entity2.HomeDoorColor != "قرمز") return 2;
            if (arg.entity1.Nationality == "دانمارک" && arg.entity3.Drink != "چای") return 3;
            //if (arg.entity2.HomeDoorColor == "سبز" && aStar.BrachNodes[2].) return 3;
            if (arg.entity2.HomeDoorColor == "سبز" && arg.entity3.Drink != "قهوه") return 3;
            if (arg.entity4.Smoking == "Pall Mall" && arg.entity5.Animal != "پرنده") return 5;
            if (arg.entity2.HomeDoorColor == "زرد" && arg.entity4.Smoking != "Dunhill") return 4;
            //if (arg.entity3.Drink== "شیر" && arg.entity4.Smoking != "Dunhill") return 4;
            // if (arg.entity1.Nationality == "نروژ" && arg.entity4.Smoking != "Dunhill") return 4;
            //if (arg.entity4.Smoking == "Blends" && arg.) return 4;
            // if (arg.entity4.Smoking == "Dunhill" && arg.) return 4;
            if (arg.entity3.Drink == "آبجو" && arg.entity4.Smoking != "Blue Master") return 4;
            if (arg.entity1.Nationality == "آلمان" && arg.entity4.Smoking != "Prince") return 4;
            //if (arg.entity3.Drink == "نروژ" && arg.entity2.HomeDoorColor != "Prince") return 4;
            //if (arg.entity4.Smoking == "Blends" && arg.entity3.Drink != "آب") return 4;

            return 0;
        }

        private void textBox1_TextChanged(object sender, TextChangedEventArgs e)
        {
            listBox1.Items.Clear();
            try
            {
                // var nums = Mahdyar_Methods.Cls_NumberRange.GetNumbers(textBox1.Text);
                // textBox1.Background = new SolidColorBrush(Colors.White);

                //foreach (var num in nums)
                //   listBox1.Items.Add(num);
            }
            catch (Exception ex)
            {
                textBox1.Background = new SolidColorBrush(Colors.Red);
            }
        }

        private void button_Click2(object sender, RoutedEventArgs e)
        {
            using(var model = new Model.dbtestEntities())
            {
                List<Agent> agents = model.Tb_AgentWorkDateStates.Select(x => new Agent() { xPersonelId_pk = x.xPersonelId_pk.Value }).Distinct().ToList();
                List<Date> dates = model.Tb_AgentWorkDateStates.Select(x => new Date() { xDate = x.xDate }).Distinct().ToList();
                List<Work> works = model.Tb_AgentWorkDateStates.Select(x => new Work() { WorkHeaderId = x.WorkHeaderId,xIdentityNo = x.xIdentityNo,
                    WorkDuration = x.WorkDurationInMinutes.Value,TimeToGoNextWorkInMinutes = x.TimeToGoNextWorkInMinutes }).Ext_DistinctBy(x=>x.WorkHeaderId).ToList();
                
                aStar2.Init(agents,works,dates);
            }
            aStar2.Evt_Promissed += AStar2_Evt_Promissed;
            j = 0;
            aStar2.SearchTree();
            MessageBox.Show(j.ToString());
        }

        int j = 0;
        private int AStar2_Evt_Promissed(AStar<Agent, Work, Date>.TreeNode arg)
        {
            j++;
            return 0;
        }
    }



    public class Cls_Door
    {
        public string HomeDoorColor { get; set; }
    }
    public class Cls_Animal
    {
        public string Animal { get; set; }
    }
    public class Cls_National
    {
        public string Nationality { get; set; }
    }
    public class Cls_Drink
    {
        public string Drink { get; set; }
    }

    public class Cls_Smoking
    {
        public string Smoking { get; set; }
    }


    public class Agent
    {
        public int xPersonelId_pk { get; set; }

        public int GetWorkCapacityInDate(Date xDate)
        {
            return 50;
        }
        public bool CanDoThisWork(Work w)
        {
            return true;
        }
    }
    public class Work
    {
        public string xIdentityNo { get; set; }
        public Nullable<long> WorkHeaderId { get; set; }
        public Nullable<int> TimeToGoNextWorkInMinutes { get; set; }
        public int WorkDuration { get; set; }
    }
    public class Date
    {
        public Nullable<System.DateTime> xDate { get; set; }
    }
}

