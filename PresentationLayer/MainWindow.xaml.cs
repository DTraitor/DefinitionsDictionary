using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
using BusinessLogicLayer;

namespace PresentationLayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DefinitionNameBox.TextChanged += (sender, args) =>
            {
                UsefulButton.IsEnabled = DefinitionNameBox.Text.Length > 0 && DefinitionMeaningBox.Text.Length > 0 && !definitionsDict.ContainsKey(DefinitionNameBox.Text);
            };
            DefinitionMeaningBox.TextChanged += (sender, args) =>
            {
                UsefulButton.IsEnabled = DefinitionNameBox.Text.Length > 0 && DefinitionMeaningBox.Text.Length > 0 && !definitionsDict.ContainsKey(DefinitionNameBox.Text);
            };
            UpdateDefinitionsList();
            Closed += (sender, args) => logic.Dispose();
        }
        
        private void UpdateDefinitionsList()
        {
            DefinitionsList.Children.Clear();
            definitionsDict.Clear();

            foreach (Definition def in logic.GetDefinitions())
            {
                Grid defPanel = new Grid();
                defPanel.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) });
                defPanel.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
                defPanel.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) });
                defPanel.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) });
                defPanel.Margin = new Thickness(2, 0, 2, 3);

                TextBlock nameTextBlock = new TextBlock() { Text = def.Name };
                nameTextBlock.Margin = new Thickness(0, 0, 3, 0);
                nameTextBlock.Width = 150;
                Grid.SetColumn(nameTextBlock, 0);

                TextBlock meaningTextBlock = new TextBlock();
                MatchCollection matches = def.DefinitionLinksRegex.Matches(def.Meaning);
                foreach (Match match in matches)
                {
                    string defName = match.Groups[2].Value;
                    string text = match.Groups[3].Value;
                    meaningTextBlock.Inlines.Add(new Run(match.Groups[1].Value));
                    if (definitionsDict.ContainsKey(defName))
                    {
                        Hyperlink link = new Hyperlink(new Run(text));
                        link.Click += (sender, args) =>
                        {
                            definitionsDict[defName].BringIntoView();
                            //Highlight the definition for a second
                            definitionsDict[defName].Background = new SolidColorBrush(Color.FromRgb(255, 255, 0));
                            Task.Delay(1000).ContinueWith(t => Dispatcher.Invoke(() =>
                            {
                                definitionsDict[defName].Background = null;
                            }));
                        };
                        meaningTextBlock.Inlines.Add(link);
                    }
                    else
                    {
                        Hyperlink link = new Hyperlink(new Run(text)) { Foreground = new SolidColorBrush(Color.FromRgb(255, 0, 0)) };
                        meaningTextBlock.Inlines.Add(link);
                    }
                }
                // Add the ending of the definition
                meaningTextBlock.Inlines.Add(new Run(def.Meaning.Substring(
                    matches.Count > 0 ? matches[matches.Count - 1].Index + matches[matches.Count - 1].Length : 0)
                ));
                meaningTextBlock.Margin = new Thickness(0, 0, 3, 0);
                Grid.SetColumn(meaningTextBlock, 1);

                Button editButton = new Button() { Content = "Редагувати" };
                editButton.Margin = new Thickness(3, 0, 3, 0);
                editButton.Click += (sender, args) =>
                {
                    currentlyEditing = def;
                    DefinitionNameBox.Text = def.Name;
                    DefinitionMeaningBox.Text = def.Meaning;
                    UsefulButton.Content = "Зберегти";
                    UsefulButton.IsEnabled = true;
                };
                Grid.SetColumn(editButton, 2);

                Button deleteButton = new Button() { Content = "Видалити" };
                deleteButton.Click += (sender, args) =>
                {
                    logic.DeleteDefinition(def);
                    UpdateDefinitionsList();
                };
                Grid.SetColumn(deleteButton, 3);

                defPanel.Children.Add(nameTextBlock);
                defPanel.Children.Add(meaningTextBlock);
                defPanel.Children.Add(editButton);
                defPanel.Children.Add(deleteButton);

                definitionsDict.Add(def.Name, defPanel);
                DefinitionsList.Children.Add(defPanel);
                DefinitionsList.Children.Add(new Separator());
            }
        }

        private void OnButtonClick(object sender, RoutedEventArgs e)
        {
            if (currentlyEditing == null)
            {
                logic.CreateDefinition(DefinitionNameBox.Text, DefinitionMeaningBox.Text);
                UpdateDefinitionsList();
                return;
            }
            if (DefinitionNameBox.Text != currentlyEditing.Name)
                logic.ChangeName(currentlyEditing, DefinitionNameBox.Text);
            if (DefinitionMeaningBox.Text != currentlyEditing.Meaning)
                logic.ChangeMeaning(currentlyEditing, DefinitionMeaningBox.Text);
            UpdateDefinitionsList();
            currentlyEditing = null;
            DefinitionNameBox.Text = "";
            DefinitionMeaningBox.Text = "";
            UsefulButton.IsEnabled = false;
            UsefulButton.Content = "Додати Визначення";
        }

        private Definition? currentlyEditing = null;
        private readonly InteractionLogic logic = new("definitions.json");
        private readonly Dictionary<string, Grid> definitionsDict = new();
    }
}