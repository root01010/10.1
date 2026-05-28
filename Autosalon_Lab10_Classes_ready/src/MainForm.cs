using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace AutosalonLab10;

public sealed class MainForm : Form
{
    private readonly List<Control> generatedControls = new();
    private readonly Label infoLabel = new();
    private readonly Button clearButton = new();
    private int clickCounter = 0;
    private readonly string logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "generated_controls_log.txt");

    public MainForm()
    {
        Text = "Лабораторна робота №10 - Класи та об'єкти";
        Size = new Size(920, 620);
        MinimumSize = new Size(760, 480);
        StartPosition = FormStartPosition.CenterScreen;
        BackColor = Color.FromArgb(245, 248, 255);
        Font = new Font("Segoe UI", 10F);

        InitializeStaticControls();
        MouseClick += MainForm_MouseClick;
        Resize += (_, _) => UpdateStatistics();
    }

    private void InitializeStaticControls()
    {
        infoLabel.Parent = this;
        infoLabel.AutoSize = false;
        infoLabel.Location = new Point(16, 16);
        infoLabel.Size = new Size(660, 72);
        infoLabel.BackColor = Color.White;
        infoLabel.BorderStyle = BorderStyle.FixedSingle;
        infoLabel.Text = "Лівий клік по формі - створити елемент.\n" +
                         "Парний елемент: кнопка Button, непарний: мітка Label.\n" +
                         "Правий клік по формі - видалити всі динамічно створені кнопки.";

        clearButton.Parent = this;
        clearButton.Text = "Очистити всі елементи";
        clearButton.Location = new Point(700, 16);
        clearButton.Size = new Size(180, 44);
        clearButton.BackColor = Color.LightSteelBlue;
        clearButton.Click += (_, _) => ClearAllGeneratedControls();
    }

    private void MainForm_MouseClick(object? sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Right)
        {
            RemoveGeneratedButtons();
            return;
        }

        if (e.Button != MouseButtons.Left)
            return;

        if (e.Y < 100)
            return; // не створюємо елементи поверх інструкції

        clickCounter++;
        if (clickCounter % 2 == 0)
            CreateDynamicButton(e.Location);
        else
            CreateDynamicLabel(e.Location);

        UpdateStatistics();
    }

    private void CreateDynamicButton(Point location)
    {
        Button button = new();
        button.Parent = this;
        button.Location = location;
        button.Size = new Size(170, 42);
        button.Text = $"Button ({location.X}; {location.Y})";
        button.BackColor = Color.FromArgb(210, 232, 255);
        button.Tag = DateTime.Now;
        button.Click += DynamicButton_Click;

        generatedControls.Add(button);
        LogAction("Створено кнопку", button);
    }

    private void CreateDynamicLabel(Point location)
    {
        Label label = new();
        label.Parent = this;
        label.Location = location;
        label.Size = new Size(180, 38);
        label.Text = $"Label ({location.X}; {location.Y})";
        label.TextAlign = ContentAlignment.MiddleCenter;
        label.BackColor = Color.FromArgb(255, 242, 204);
        label.BorderStyle = BorderStyle.FixedSingle;
        label.Tag = DateTime.Now;
        label.DoubleClick += (_, _) =>
        {
            label.BackColor = Color.FromArgb(220, 255, 220);
            label.Text = "Мітку змінено";
        };

        generatedControls.Add(label);
        LogAction("Створено мітку", label);
    }

    private void DynamicButton_Click(object? sender, EventArgs e)
    {
        // Демонстрація safe cast через is/as.
        if (sender is Button)
        {
            Button? button = sender as Button;
            if (button != null)
            {
                button.Text = "Натиснуто!";
                button.BackColor = Color.FromArgb(198, 239, 206);
                LogAction("Натиснуто кнопку", button);
            }
        }
        UpdateStatistics();
    }

    private void RemoveGeneratedButtons()
    {
        var buttonsToRemove = generatedControls.Where(c => c is Button).ToList();
        foreach (Control control in buttonsToRemove)
        {
            generatedControls.Remove(control);
            Controls.Remove(control);
            control.Dispose();
        }

        LogText($"Видалено кнопок: {buttonsToRemove.Count}");
        UpdateStatistics();
    }

    private void ClearAllGeneratedControls()
    {
        foreach (Control control in generatedControls.ToList())
        {
            Controls.Remove(control);
            control.Dispose();
        }
        generatedControls.Clear();
        LogText("Очищено всі динамічні елементи");
        UpdateStatistics();
    }

    private void UpdateStatistics()
    {
        int buttons = generatedControls.Count(c => c is Button);
        int labels = generatedControls.Count(c => c is Label);
        Text = $"ЛР №10 - Класи та об'єкти | Button: {buttons}, Label: {labels}, Всього: {generatedControls.Count}";
    }

    private void LogAction(string action, Control control)
    {
        LogText($"{action}: {control.GetType().Name}, Text='{control.Text}', Location=({control.Left}; {control.Top}), Size=({control.Width}x{control.Height})");
    }

    private void LogText(string text)
    {
        File.AppendAllText(logPath, $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {text}{Environment.NewLine}");
    }
}
