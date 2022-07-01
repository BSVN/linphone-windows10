/*
AddressBox.xaml.cs
Copyright (C) 2022  Resa Co.
Copyright (C) 2016  Belledonne Communications, Grenoble, France
This program is free software; you can redistribute it and/or
modify it under the terms of the GNU General Public License
as published by the Free Software Foundation; either version 2
of the License, or (at your option) any later version.
This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.
You should have received a copy of the GNU General Public License
along with this program; if not, write to the Free Software
Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
*/

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Linphone.Controls
{

    public partial class AddressBox : UserControl
    {
        public AddressBox()
        {
            InitializeComponent();
            this.DataContext = this;
        }

		/// <summary>
		/// The <see cref="DependencyProperty"/> backing <see cref="Text"/>.
		/// </summary>
		public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
			nameof(Text),
			typeof(string),
			typeof(AddressBox),
            new PropertyMetadata(default(string)));


		/// <summary>
		/// Gets or sets the <see cref="string"/> representing the text to display.
		/// </summary>
		public string Text
		{
            get
            {
                return (string)GetValue(TextProperty);
            }
            set
            {
                if(value.Length > 0)
                {
                    Backspace.IsEnabled = true;
                }
                else
                {
                    Backspace.IsEnabled = false;
                }
                SetValue(TextProperty, value);
            }
		}

        private void Backspace_Hold(object sender, RoutedEventArgs e)
		{
            Text = string.Empty;
		}

        private void Backspace_Click(object sender, RoutedEventArgs e)
        {
            if (Text.Length > 0)
                Text = Text.Substring(0, Text.Length - 1);
        }

        private void address_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(Text.Length > 0)
            {
                Backspace.IsEnabled = true;
            } else
            {
                Backspace.IsEnabled = false;
            }
        }
    }
}
