using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Frontend.Propriedades.WPF
{
    public class MaskedTextBox : TextBox
    {
        public static DependencyProperty MaskProperty;

        static MaskedTextBox()
        {
            MaskProperty = DependencyProperty.Register("Mask", typeof(string), typeof(MaskedTextBox), new FrameworkPropertyMetadata(MaskChanged));
            FrameworkPropertyMetadata metadata = new FrameworkPropertyMetadata
            {
                CoerceValueCallback = CoerceText
            };
            TextProperty.OverrideMetadata(typeof(MaskedTextBox), metadata);
            CommandManager.RegisterClassCommandBinding(typeof(MaskedTextBox), new CommandBinding(ApplicationCommands.Paste, null));


        }

        private static void MaskChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            d.CoerceValue(TextProperty);
        }

        private static object CoerceText(DependencyObject d, object value)
        {
            MaskedTextBox textBox = (MaskedTextBox)d;
            MaskedTextProvider maskProvider = new MaskedTextProvider(textBox.Mask)
            {
                PromptChar = ' '
            };
            maskProvider.Set((string)value ?? string.Empty);

            return maskProvider.ToDisplayString();
        }

        public MaskedTextBox() : base()
        {
            CommandBinding commandBindingPaste = new CommandBinding(ApplicationCommands.Paste, null, this.SuppressCommand);
            this.CommandBindings.Add(commandBindingPaste);
            CommandBinding commandBindingCut = new CommandBinding(ApplicationCommands.Cut, null, this.SuppressCommand);
            this.CommandBindings.Add(commandBindingCut);
        }

        private void SuppressCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = false;
            e.Handled = true;
        }

        public string Mask
        {
            get => (string)this.GetValue(MaskProperty);
            set => this.SetValue(MaskProperty, value);
        }

        public bool MaskCompleted
        {
            get
            {
                MaskedTextProvider maskProvider = this.GetMaskProvider();
                return maskProvider.MaskCompleted;
            }
        }

        protected override void OnGotFocus(RoutedEventArgs e)
        {
            int pos = this.Text.Equals(this.GetMaskProvider().ToDisplayString()) ? 0 : this.SkipToEditableCharacter(this.SelectionStart);
            this.Dispatcher.BeginInvoke(new Action(() => this.Select(pos, 0)));
        }

        protected override void OnMouseDoubleClick(MouseButtonEventArgs e)
        {
            base.OnMouseDoubleClick(e);
            this.SelectAll();
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            MaskedTextProvider maskProvider = this.GetMaskProvider();
            int pos = this.SelectionStart;
            // Delete
            if (e.Key == Key.Delete && pos < this.Text.Length)
            {
                if (string.IsNullOrEmpty(this.SelectedText))
                {
                    if (maskProvider.RemoveAt(pos))
                    {
                        this.RefreshText(maskProvider, pos);
                    }
                }
                else if (maskProvider.RemoveAt(pos, pos + this.SelectionLength - 1))
                {
                    this.RefreshText(maskProvider, pos);
                }
                e.Handled = true;
            }
            // Backspace
            else if (e.Key == Key.Back)
            {
                if (!string.IsNullOrEmpty(this.SelectedText))
                {
                    if (maskProvider.RemoveAt(pos, pos + this.SelectionLength - 1))
                    {
                        this.RefreshText(maskProvider, pos);
                    }
                }
                if (pos > 0)
                {
                    pos--;
                    if (maskProvider.RemoveAt(pos))
                    {
                        this.RefreshText(maskProvider, pos);
                    }
                }
                e.Handled = true;
            }
            else if (e.Key == Key.Space || e.Key == Key.Decimal)
            {
                int max = maskProvider.Length - maskProvider.EditPositionCount;
                int nextField = 0;
                for (int i = 0; i < maskProvider.Length; i++)
                {
                    if (!maskProvider.IsEditPosition(i) && (pos + max) >= i)
                        nextField = i;
                }
                this.RefreshText(maskProvider, nextField);
                e.Handled = true;
            }
            else if (e.Key >= Key.D0 && e.Key <= Key.D9)
            {
                if (this.SelectionLength == maskProvider.Length)
                {
                    if (maskProvider.RemoveAt(pos, pos + this.SelectionLength - 1))
                    {
                        this.RefreshText(maskProvider, pos);
                    }
                }
            }
        }

        protected override void OnPreviewTextInput(TextCompositionEventArgs e)
        {
            MaskedTextProvider maskProvider = this.GetMaskProvider();
            int pos = this.SelectionStart;
            this.RefreshText(maskProvider, pos);
            e.Handled = true;
            base.OnPreviewTextInput(e);
            if (pos < this.Text.Length)
            {
                pos = this.SkipToEditableCharacter(pos);
                if (Keyboard.IsKeyToggled(Key.Insert))
                {
                    if (maskProvider.Replace(e.Text, pos))
                    {
                        pos++;
                    }
                }
                else
                {
                    if (maskProvider.InsertAt(e.Text, pos))
                    {
                        pos++;
                    }
                }
                pos = this.SkipToEditableCharacter(pos);
            }
            this.RefreshText(maskProvider, pos);
            e.Handled = true;
            base.OnPreviewTextInput(e);
        }

        private void RefreshText(MaskedTextProvider maskProvider, int pos)
        {
            this.Text = maskProvider.ToDisplayString();
            this.SelectionStart = pos;
        }

        private MaskedTextProvider GetMaskProvider()
        {
            MaskedTextProvider maskProvider = new MaskedTextProvider(this.Mask)
            {
                PromptChar = ' '
            };
            maskProvider.Set(this.Text);
            return maskProvider;
        }

        private int SkipToEditableCharacter(int startPos)
        {
            MaskedTextProvider maskProvider = this.GetMaskProvider();
            int newPos = maskProvider.FindEditPositionFrom(startPos, true);
            return newPos == -1 ? startPos : newPos;
        }
    }
}
