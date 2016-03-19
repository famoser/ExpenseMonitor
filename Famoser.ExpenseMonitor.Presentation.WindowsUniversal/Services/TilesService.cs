using System;
using System.Linq;
using Windows.UI.Notifications;
using Famoser.ExpenseMonitor.View.Enums;
using Famoser.ExpenseMonitor.View.ViewModel;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using NotificationsExtensions.Tiles;

namespace Famoser.ExpenseMonitor.Presentation.WindowsUniversal.Services
{
    public class TilesService
    {
        public TilesService()
        {
            Messenger.Default.Register<Messages>(this, EvaluateMessages);
        }

        private void EvaluateMessages(Messages obj)
        {
            if (obj == Messages.ExpenseChanged)
            {
                var vm = SimpleIoc.Default.GetInstance<MainViewModel>();
                if (vm != null && vm.ExpenseCollections != null)
                {
                    var newNotes =vm.ExpenseCollections.SelectMany(noteCollectionModel => noteCollectionModel.Expenses).ToList();
                    var adap2 = new TileBindingContentAdaptive()
                    {
                        Children =
                        {
                            new TileText()
                            {
                                Style = TileTextStyle.Body,
                                Text = newNotes.Sum(n => n.Amount).ToString("0.##") + " total spent"
                            },
                            // For spacing
                            new TileText()
                            {
                                Style = TileTextStyle.Body,
                                Text = newNotes.Where(n => n.CreateTime > DateTime.Now.Subtract(TimeSpan.FromDays(7))).Sum(n => n.Amount).ToString("0.##") + " spent last 7 days"
                            },
                            // For spacing
                            new TileText()
                            {
                                Style = TileTextStyle.Body,
                                Text = newNotes.Where(n => n.CreateTime > DateTime.Now.Subtract(DateTime.Now - DateTime.Today)).Sum(n => n.Amount).ToString("0.##") + " spent today"
                            },
                        }
                    };

                    var tileLarge = new TileBinding()
                    {
                        Branding = TileBranding.None,
                        Content = adap2
                    };

                    var tileSmall = new TileBinding()
                    {
                        Branding = TileBranding.None,
                        Content = new TileBindingContentAdaptive()
                        {
                            Children =
                            {
                                new TileText()
                                {
                                    Style = TileTextStyle.Header,
                                    Text = newNotes.Count.ToString("00"),
                                    Align = TileTextAlign.Center
                                }
                            }
                        }
                    };

                    var tileVisual = new TileVisual()
                    {
                        TileLarge = tileLarge,
                        TileMedium = tileLarge,
                        TileWide = tileLarge,
                        TileSmall = tileSmall,
                    };

                    var tileContent = new TileContent()
                    {
                        Visual = tileVisual
                    };

                    var notif = new TileNotification(tileContent.GetXml());
                    var updater = TileUpdateManager.CreateTileUpdaterForApplication();
                    updater.Update(notif);
                }
            }
        }
    }
}
