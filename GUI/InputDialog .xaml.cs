using System.Windows;
using System.Windows.Controls;

namespace KostPostMusic;

public class InputDialog : Window
{
    public string ResponseText
    {
        get { return ResponseTextBox.Text; }
        set { ResponseTextBox.Text = value; }
    }

    private TextBox ResponseTextBox;

    public InputDialog(string title, string question)
    {
        Title = title;
        Width = 300;
        Height = 150;
        WindowStartupLocation = WindowStartupLocation.CenterOwner;

        var grid = new Grid();
        Content = grid;

        grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });
        grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });
        grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });

        var questionLabel = new Label { Content = question, Margin = new Thickness(5) };
        grid.Children.Add(questionLabel);
        Grid.SetRow(questionLabel, 0);

        ResponseTextBox = new TextBox { Margin = new Thickness(5) };
        grid.Children.Add(ResponseTextBox);
        Grid.SetRow(ResponseTextBox, 1);

        var buttonsPanel = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Right, Margin = new Thickness(5) };
        grid.Children.Add(buttonsPanel);
        Grid.SetRow(buttonsPanel, 2);

        var okButton = new Button { Content = "OK", Width = 75, Margin = new Thickness(5) };
        okButton.Click += (sender, e) => { DialogResult = true; };
        buttonsPanel.Children.Add(okButton);

        var cancelButton = new Button { Content = "Cancel", Width = 75, Margin = new Thickness(5) };
        cancelButton.Click += (sender, e) => { DialogResult = false; };
        buttonsPanel.Children.Add(cancelButton);
    }
}