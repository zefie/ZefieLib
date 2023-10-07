﻿using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace ZefieLib
{
    public class Controls
    {
        public static void ActivateForm(Form f)
        {
            if (f.InvokeRequired) _ = f.Invoke(new MethodInvoker(delegate { f.Activate(); }));
            else f.Activate();
        }

        public static void SetComboBoxIndex(ComboBox cb, int index)
        {
            if (cb.InvokeRequired) _ = cb.Invoke(new MethodInvoker(delegate { cb.SelectedIndex = index; }));
            else cb.SelectedIndex = index;
        }

        public static void SetButtonEnabled(Button b, bool enabled)
        {
            if (b.InvokeRequired) _ = b.Invoke(new MethodInvoker(delegate { b.Enabled = enabled; }));
            else b.Enabled = enabled;
        }

        public static void SetButtonImage(Button b, Image image)
        {
            if (b.InvokeRequired)
            {
                _ = b.Invoke(new MethodInvoker(delegate
                  {
                      b.Image.Dispose();
                      b.Image = image;
                  }));
            }
            else
            {
                b.Image.Dispose();
                b.Image = image;
            }
        }

        public static void SetLabelText(Label l, string text)
        {
            if (l.InvokeRequired) _ = l.Invoke(new MethodInvoker(delegate { l.Text = text; }));
            else l.Text = text;
        }

        public static void SetFocus(Control ctrl)
        {
            if (ctrl.InvokeRequired) _ = ctrl.Invoke(new MethodInvoker(delegate { ctrl.Focus(); }));
            else ctrl.Focus();
        }

        public static void SetTop(Control ctrl, int top)
        {
            if (ctrl.InvokeRequired) _ = ctrl.Invoke(new MethodInvoker(delegate { ctrl.Top = top; }));
            else ctrl.Top = top;
        }

        public static void SetLeft(Control ctrl, int left)
        {
            if (ctrl.InvokeRequired) _ = ctrl.Invoke(new MethodInvoker(delegate { ctrl.Left = left; }));
            else ctrl.Left = left;
        }

        public static void SetColor(Control ctrl, Color color)
        {
            if (ctrl.InvokeRequired) _ = ctrl.Invoke(new MethodInvoker(delegate { ctrl.ForeColor = color; }));
            else ctrl.ForeColor = color;
        }
        public static void SetBackColor(Control ctrl, Color color)
        {
            if (ctrl.InvokeRequired) _ = ctrl.Invoke(new MethodInvoker(delegate { ctrl.BackColor = color; }));
            else ctrl.BackColor = color;
        }

        public static void SetLabelText(ToolStripStatusLabel l, string text)
        {
            if (l.GetCurrentParent().InvokeRequired) _ = l.GetCurrentParent().Invoke(new MethodInvoker(delegate { l.Text = text; }));
            else l.Text = text;
        }

        public static void SetTextboxText(RichTextBox rtb, string text)
        {
            if (rtb.InvokeRequired) _ = rtb.Invoke(new MethodInvoker(delegate { rtb.Text = text; }));
            else rtb.Text = text;
        }

        public static void SetTextboxText(TextBox tb, string text)
        {
            if (tb.InvokeRequired) _ = tb.Invoke(new MethodInvoker(delegate { tb.Text = text; }));
            else tb.Text = text;
        }

        public static void SetTextboxSelection(TextBox tb, int start, int length)
        {
            if (tb.InvokeRequired) _ = tb.Invoke(new MethodInvoker(delegate { tb.Select(start, length); }));
            else tb.Select(start, length);
        }

        public static void SetTextboxSelection(RichTextBox rtb, int start, int length)
        {
            if (rtb.InvokeRequired) _ = rtb.Invoke(new MethodInvoker(delegate { rtb.Select(start, length); }));
            else rtb.Select(start, length);
        }

        public static int GetTextboxTextLength(TextBox tb)
        {
            int l = 0;
            if (tb.InvokeRequired) _ = tb.Invoke(new MethodInvoker(delegate { l = tb.TextLength; }));
            else l = tb.TextLength;
            return l;
        }

        public static int GetTextboxTextLength(RichTextBox rtb)
        {
            int l = 0;
            if (rtb.InvokeRequired) _ = rtb.Invoke(new MethodInvoker(delegate { l = rtb.TextLength; }));
            else l = rtb.TextLength;
            return l;
        }

        public static void AppendTextboxText(RichTextBox rtb, string text)
        {
            if (rtb.InvokeRequired) _ = rtb.Invoke(new MethodInvoker(delegate { rtb.AppendText(text); }));
            else rtb.AppendText(text);
        }

        public static void AppendTextboxText(TextBox tb, string text)
        {
            if (tb.InvokeRequired) _ = tb.Invoke(new MethodInvoker(delegate { tb.AppendText(text); }));
            else tb.AppendText(text);
        }

        public static void TextBoxScrollToCaret(TextBox tb)
        {
            if (tb.InvokeRequired) _ = tb.Invoke(new MethodInvoker(delegate { tb.ScrollToCaret(); }));
            else tb.ScrollToCaret();
        }

        public static void TextBoxScrollToCaret(RichTextBox rtb)
        {
            if (rtb.InvokeRequired) _ = rtb.Invoke(new MethodInvoker(delegate { rtb.ScrollToCaret(); }));
            else rtb.ScrollToCaret();
        }

        public static string GetLabelText(Label l)
        {
            string value = "";
            if (l.InvokeRequired) _ = l.Invoke(new MethodInvoker(delegate { value = l.Text; }));
            else value = l.Text;
            return value;
        }

        public static void SetTrackbarValue(TrackBar t, int value)
        {
            if (value <= t.Maximum && value >= t.Minimum)
            {
                if (t.InvokeRequired) _ = t.Invoke(new MethodInvoker(delegate { t.Value = value; }));
                else t.Value = value;
            }
        }

        public static void SetProgressbarValue(ProgressBar t, int value)
        {
            if (value <= t.Maximum && value >= t.Minimum)
            {
                if (t.InvokeRequired) _ = t.Invoke(new MethodInvoker(delegate { t.Value = value; }));
                else t.Value = value;
            }
        }

        public static void SetProgressbarValue(ProgressBar t, int value, int max)
        {
            if (t.InvokeRequired)
            {
                _ = t.Invoke(new MethodInvoker(delegate
                {
                    t.Maximum = max;
                    t.Value = value;
                }));
            }
            else
            {
                t.Maximum = max;
                t.Value = value;
            }
        }

        public static int GetTrackbarValue(TrackBar t)
        {
            int value = -1;
            if (t.InvokeRequired) _ = t.Invoke(new MethodInvoker(delegate { value = t.Value; }));
            else value = t.Value;
            return value;
        }

        public static int GetComboBoxIndex(ComboBox t)
        {
            int value = -1;
            if (t.InvokeRequired) _ = t.Invoke(new MethodInvoker(delegate { value = t.SelectedIndex; }));
            else value = t.SelectedIndex;
            return value;
        }
        public static string GetComboBoxText(ComboBox t)
        {
            string value = "";
            if (t.InvokeRequired) _ = t.Invoke(new MethodInvoker(delegate { value = t.Text; }));
            else value = t.Text;
            return value;
        }
        public static bool GetCheckBoxChecked(CheckBox t)
        {
            bool value = false;
            if (t.InvokeRequired) _ = t.Invoke(new MethodInvoker(delegate { value = t.Checked; }));
            else value = t.Checked;
            return value;
        }

        public static void SetCheckBoxChecked(CheckBox c, bool @checked)
        {
            if (c.InvokeRequired)  _ = c.Invoke(new MethodInvoker(delegate { c.Checked = @checked; }));
            else c.Checked = @checked;
        }

        public static void SetControlVisiblity(Control c, bool visible)
        {
            if (c.InvokeRequired) _ = c.Invoke(new MethodInvoker(delegate { c.Visible = visible; }));
            else c.Visible = visible;
        }

        public static void SetControlEnabled(Control c, bool enabled)
        {
            if (c.InvokeRequired) _ = c.Invoke(new MethodInvoker(delegate { c.Enabled = enabled; }));
            else c.Enabled = enabled;
        }

        public static void SetControlSize(Control c, Size s)
        {
            if (c.InvokeRequired) _ = c.Invoke(new MethodInvoker(delegate { c.Size = s; }));
            else c.Size = s;
        }

        public static Size GetControlSize(Control c)
        {
            Size s = new Size();
            if (c.InvokeRequired) _ = c.Invoke(new MethodInvoker(delegate { s = c.Size; }));
            else s = c.Size;
            return s;
        }

        public static Size GetControlClientSize(Control c)
        {
            Size s = new Size();
            if (c.InvokeRequired) _ = c.Invoke(new MethodInvoker(delegate { s = c.ClientSize; }));
            else s = c.ClientSize;
            return s;
        }

        public static Padding GetControlPadding(Control c)
        {
            Padding p = new Padding();
            if (c.InvokeRequired) _ = c.Invoke(new MethodInvoker(delegate { p = c.Padding; }));
            else p = c.Padding;
            return p;
        }

        public static Padding GetControlMargin(Control c)
        {
            Padding p = new Padding();
            if (c.InvokeRequired) _ = c.Invoke(new MethodInvoker(delegate { p = c.Margin; }));
            else p = c.Margin;
            return p;
        }

        public static void ClickButton(Button ctrl)
        {
            if (ctrl.InvokeRequired) _ = ctrl.Invoke(new MethodInvoker(delegate { ctrl.PerformClick(); }));
            else ctrl.PerformClick();
        }

        public static bool IsFocused(Control ctrl)
        {
            bool focused = false;
            if (ctrl.InvokeRequired) _ = ctrl.Invoke(new MethodInvoker(delegate { focused = ctrl.Focused; }));
            else focused = ctrl.Focused;
            return focused;
        }

        public static int GetActiveIndex(Panel panel)
        {
            int StoredIndex = -1;
            foreach (Control? obj in panel.Controls)
            {
                if (obj == null) continue;
                if (IsFocused(obj))
                {
                    StoredIndex = panel.Controls.IndexOf(obj);
                    break;
                }
            }
            return StoredIndex;
        }

        public static int GetActiveIndex(Form form)
        {
            int StoredIndex = -1;
            foreach (Control? obj in form.Controls)
            {
                if (obj == null) continue;
                if (IsFocused(obj))
                {
                    StoredIndex = form.Controls.IndexOf(obj);
                    break;
                }
            }
            return StoredIndex;
        }

        public class Custom
        {
            public class ColorProgressBar : ProgressBar
            {
                public Color[] Colors = new Color[2];
                public LinearGradientMode GradientMode = LinearGradientMode.Vertical;
                public int inset = 2; // A single inset value to control the sizing of the inner rect.
                private readonly bool IsWinXP = System.Environment.OSVersion.Version.Major <= 5; // or older

                public ColorProgressBar()
                {
                    if (!IsWinXP)
                    {
                        SetStyle(ControlStyles.UserPaint, true);
                        Colors = new Color[2]
                        {
                            BackColor,
                            ForeColor
                        };
                    }
                }

                protected override void OnPaintBackground(PaintEventArgs pevent)
                {
                    if (IsWinXP)
                    {
                        base.OnPaintBackground(pevent);
                        return;
                    }
                    // None... Helps control the flicker.
                }

                protected override void OnPaint(PaintEventArgs e)
                {
                    if (IsWinXP)
                    {
                        base.OnPaint(e);
                        return;
                    }

                    using (Image offscreenImage = new Bitmap(Width, Height))
                    {
                        using (Graphics offscreen = Graphics.FromImage(offscreenImage))
                        {
                            Rectangle rect = new Rectangle(0, 0, Width, Height);

                            if (ProgressBarRenderer.IsSupported)
                            {
                                ProgressBarRenderer.DrawHorizontalBar(offscreen, rect);
                            }

                            rect.Inflate(new Size(-inset, -inset)); // Deflate inner rect.
                            rect.Width = (int)(rect.Width * ((double)Value / Maximum));
                            if (rect.Width == 0)
                            {
                                rect.Width = 1; // Can't draw rec with width of 0.
                            }

                            LinearGradientBrush brush = new LinearGradientBrush(rect, Colors[0], Colors[1], GradientMode);
                            offscreen.FillRectangle(brush, inset, inset, rect.Width, rect.Height);

                            e.Graphics.DrawImage(offscreenImage, 0, 0);
                            offscreenImage.Dispose();
                        }
                    }
                }
            }
        }
    }
}
