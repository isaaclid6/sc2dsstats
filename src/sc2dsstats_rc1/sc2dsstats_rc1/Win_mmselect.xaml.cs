﻿using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace sc2dsstats_rc1
{
    /// <summary>
    /// Interaktionslogik für Win_mmselect.xaml
    /// </summary>
    public partial class Win_mmselect : Window
    {

        MainWindow MW { get; set; }
        Win_mm WM { get; set; }

        public Win_mmselect()
        {
            InitializeComponent();
            ContextMenu dg_games_cm = new ContextMenu();
            MenuItem win_saveas = new MenuItem();
            win_saveas.Header = "Copy result ...";
            win_saveas.Click += new RoutedEventHandler(dg_games_cm_Copy_Click);
            dg_games_cm.Items.Add(win_saveas);
            dg_games.ContextMenu = dg_games_cm;
            dg_games.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(dg_games_DClick);

        }

        public Win_mmselect(MainWindow mw, Win_mm wm)  : this()
        {
            MW = mw;
            WM = wm;

            bt_load_Click(null, null);
        }

        private void bt_scan_Click(object sender, RoutedEventArgs e)
        {
            MW.mnu_Scan(null, null);
        }

        private void bt_load_Click(object sender, RoutedEventArgs e)
        {
            var today = DateTime.Now;
            var yesterday = today.AddDays(-2);
            string sd = yesterday.ToString("yyyyMMdd");
            sd += "000000";
            double sd_int = double.Parse(sd);

            MW.LoadData(MW.myStats_csv);
            List<dsreplay> fil_replays = new List<dsreplay>(MW.replays);
            List<dsreplay> tmprep = new List<dsreplay>();
            tmprep = new List<dsreplay>(fil_replays.Where(x => (x.GAMETIME > sd_int)).ToList());

            dg_games.ItemsSource = tmprep;
        }

        private void bt_send_Click(object sender, RoutedEventArgs e)
        {
            string result1 = "";
            string result2 = "";
            string result = "unknown";

            foreach (var dataItem in dg_games.SelectedItems)
            {
                dsreplay game = dataItem as dsreplay;
                if (game != null)
                {

                    foreach (dsplayer player in game.PLAYERS)
                    {
                        if (player.POS <= 3)
                        {
                            result1 += "(" + player.NAME + ", " + player.RACE + ", " + player.KILLSUM + ")";
                            if (player.POS != 3)
                            {
                                result1 += ", ";
                            }
                        }
                        else if (player.POS > 3)
                        {
                            result2 += "(" + player.NAME + ", " + player.RACE + ", " + player.KILLSUM + ")";
                            if (player.POS != 6)
                            {
                                result2 += ", ";
                            }

                        }
                    }
                }

                if (game.WINNER == 0)
                {
                    result = result1 + " vs " + result2;
                }
                else if (game.WINNER == 1)
                {
                    result = result2 + " vs " + result1;
                }
                result1 = "";
                result2 = "";

            }
            WM.SendResult("mmid: " + WM.tb_mmid.Text + "; result: " + result);

        }

        private void dg_games_DClick(object sender, RoutedEventArgs e)
        {
            List<dsplayer> temp = new List<dsplayer>();
            dsplayer pltemp = new dsplayer();
            foreach (var dataItem in dg_games.SelectedItems)
            {
                //myGame game = dataItem as myGame;
                dsreplay game = dataItem as dsreplay;
                pltemp.RACE = game.REPLAY;
                temp.Add(pltemp);
                foreach (dsplayer pl in game.PLAYERS)
                {
                    temp.Add(pl);
                }

            }

            if (temp.Count > 300)
            {
                pltemp.RACE = "Visibility ilmit is 300. Sorry.";
                List<dsplayer> bab = new List<dsplayer>();
                bab.Add(pltemp);

                dg_player.ItemsSource = bab;
            }
            else
            {
                dg_player.ItemsSource = temp;
            }

            if (temp.Count < 120)
            {
                dg_player.EnableRowVirtualization = false;

                Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.ApplicationIdle, new Action(ProcessRows_player));
            }
            //ProcessRows_player();
        }

        private void ProcessRows_player()
        {

            int itct = dg_player.Items.Count;
            for (int i = 0; i < itct; i++)
            {
                ///var row = dg_player.ItemContainerGenerator.ContainerFromItem(pl) as DataGridRow;

                dsplayer pl = dg_player.Items[i] as dsplayer;

                var row = dg_player.ItemContainerGenerator.ContainerFromIndex(i) as DataGridRow;
                if (row != null)
                {
                    if (MW.player_list.Contains(pl.NAME))
                    {
                        row.Background = Brushes.YellowGreen;
                    }
                    else if (pl.NAME == null)
                    {
                        row.Background = Brushes.Azure;
                    }
                    else
                    {
                        row.Background = Brushes.Yellow;
                    }
                }
            }

        }

        private void dg_games_cm_Copy_Click(object sender, RoutedEventArgs e)
        {
            string result1 = "";
            string result2 = "";
            string result = "unknown";

            if (dg_games.SelectedItems.Count > 100)
            {
                MessageBox.Show("Too many replays to handle. We can only handle 100 with this :(", "Failed");
            }
            else
            {

                foreach (dsreplay game in dg_games.SelectedItems)
                {
                    if (game != null)
                    {

                        foreach (dsplayer player in game.PLAYERS)
                        {
                            if (player.POS <= 3)
                            {
                                result1 += "(" + player.NAME + ", " + player.RACE + ", " + player.KILLSUM + ")";
                                if (player.POS != 3)
                                {
                                    result1 += ", ";
                                }
                            }
                            else if (player.POS > 3)
                            {
                                result2 += "(" + player.NAME + ", " + player.RACE + ", " + player.KILLSUM + ")";
                                if (player.POS != 6)
                                {
                                    result2 += ", ";
                                }

                            }
                        }
                    }

                    if (game.WINNER == 0)
                    {
                        result = result1 + " vs " + result2;
                    }
                    else if (game.WINNER == 1)
                    {
                        result = result2 + " vs " + result1;
                    }
                    result1 = "";
                    result2 = "";

                }
                Clipboard.SetText(result);
                MessageBox.Show(result, "Sent to clipboard");
            }
        }

        private void bt_norep_Click(object sender, RoutedEventArgs e)
        {
            Win_norep norep = new Win_norep();
            norep.Show();
        }
    }
}
