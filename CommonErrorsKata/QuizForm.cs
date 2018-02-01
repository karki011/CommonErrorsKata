using CommonErrorsKata.Shared;
using System.IO;
using System.Linq;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CommonErrorsKata
{
    public partial class CommonErrorsForm : Form
    {
        private readonly AnswerQueue<TrueFalseAnswer> answerQueue;
        private readonly string[] files;
        private readonly SynchronizationContext synchronizationContext;
        private int _time = 100;
        private string currentBaseName;
        private readonly string[] possibleAnswers;
        bool IsPlaying = true;

        public CommonErrorsForm()
        {
            InitializeComponent();
            synchronizationContext = SynchronizationContext.Current;
            files = System.IO.Directory.GetFiles(Environment.CurrentDirectory + @"..\..\..\ErrorPics");
            possibleAnswers = files.Select(f => Path.GetFileNameWithoutExtension(f)).ToArray();
            lstAnswers.DataSource = possibleAnswers;
            answerQueue = new AnswerQueue<TrueFalseAnswer>(possibleAnswers.Length);
            Next();
            lstAnswers.Click += LstAnswers_Click;
            StartTimer();
        }

        private async void StartTimer()
        {
            await Task.Run(() =>
            {
                for (_time = 100; _time > 0; _time--)
                {
                    UpdateProgress(_time);
                    Thread.Sleep(50);
                }

                if (IsPlaying == false)
                {
                    Application.Exit();
                }
                Message("Need to be quicker on your feet next time!  Try again...");
            });
        }

        private void LstAnswers_Click(object sender, EventArgs e)
        {
            _time = 100;
            var selected = possibleAnswers[lstAnswers.SelectedIndex];
            answerQueue.Enqueue(new TrueFalseAnswer(selected == currentBaseName));
            Next();
        }

        private void Next()
        {
            if (answerQueue.Count == possibleAnswers.Length && answerQueue.Grade >= 98)
            {
                IsPlaying = false;
                MessageBox.Show("Congratulations you've defeated me!");
                Application.Exit();
                return;
            }

            label1.Text = answerQueue.Grade.ToString() + "%";
            var file = files.GetRandom();
            currentBaseName = Path.GetFileNameWithoutExtension(file);
            pbImage.ImageLocation = file;
        }

        public void UpdateProgress(int value)
        {
            synchronizationContext.Post(new SendOrPostCallback(x =>
            {
                progress.Value = value;
            }), value);
        }

        public void Message(string value)
        {
            synchronizationContext.Post(new SendOrPostCallback(x =>
            {
                MessageBox.Show(value);
            }), value);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
