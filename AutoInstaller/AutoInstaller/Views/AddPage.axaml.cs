using Avalonia.Controls;
using Avalonia.Styling;

namespace AutoInstaller.Views
{
    public partial class AddPage : UserControl
    {
        public AddPage()
        {
            InitializeComponent();

            //Style DisabledTextBoxStyle = new Style(x => x.OfType<Border>().Descendant().OfType<TextBox>()
	           // .PropertyEquals(TextBox.IsEnabledProperty, false));

            //Setter errorTemplateSetter = new Setter();
            //errorTemplateSetter.Property = DataValidationErrors.ErrorTemplateProperty;
            //errorTemplateSetter.Value = null;

            //Setter templateSetter = new Setter();
            //templateSetter.Property = DataValidationErrors.TemplateProperty.;

            //Styles.Add(DisabledTextBoxStyle);
        }
    }
}
