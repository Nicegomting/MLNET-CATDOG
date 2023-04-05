using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MLNET
{
    public partial class MLForm : Form
    {
        public MLForm()
        {
            InitializeComponent();
            KeyPreview = true;
        }

        private void MLForm_Load(object sender, EventArgs e)
        {
        }

        private void btnFileSelect_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Image File|*.jpg;*.jpeg;*.png;*.gif;";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    txtPath.Text = openFileDialog.FileName;
                    picPreview.ImageLocation = openFileDialog.FileName;

                    var result = IsCATorDOG(txtPath.Text);
                    if(result.Item1 is null)
                        lblResult.Text = $"알 수 없는 유형입니다.";
                    else if(result.Item1 == true)
                        lblResult.Text = $"This is a {result.Item2*100.00:F2}% Cat.";
                    else
                        lblResult.Text = $"This is a {result.Item2*100.00:F2}% Dog.";
                }
            }
        }

        private Tuple<bool?, float> IsCATorDOG(string path)
        {
            try
            {
                if (!File.Exists(path))
                    return null;
            }
            catch
            {
                return null;
            }

            var imageBytes = File.ReadAllBytes(path);
            MLModel.ModelInput sampleData = new MLModel.ModelInput()
            {
                ImageSource = imageBytes,
            };

            var result = MLModel.Predict(sampleData);
            float score = result.Score.Max();
            bool? isCAT = result.PredictedLabel.StartsWith("cat");
            return new Tuple<bool?, float>(isCAT, score);
        }
    }
}
