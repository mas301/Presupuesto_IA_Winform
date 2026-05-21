using System;
using System.IO;
using System.Windows.Forms;

namespace DevExpressTreeListDemo
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.ThreadException += OnThreadException;
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;

            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                // Verificar si ya hay una instancia en ejecución
                if (System.Diagnostics.Process.GetProcessesByName(System.IO.Path.GetFileNameWithoutExtension(System.Reflection.Assembly.GetEntryAssembly().Location)).Length > 1)
                {
                    DialogResult result = MessageBox.Show("Ya hay una instancia en ejecución. ¿Desea continuar?", "Advertencia", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (result == DialogResult.No)
                    {
                        return;
                    }
                }

                Application.Run(new MainForm());
            }
            catch (Exception ex)
            {
                ReportError("Startup exception", ex);
            }
        }

        private static void OnThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            ReportError("UI thread exception", e.Exception);
        }

        private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = e.ExceptionObject as Exception ?? new Exception("Unknown unhandled exception.");
            ReportError("Unhandled exception", ex);
        }

        private static void ReportError(string title, Exception ex)
        {
            string logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "startup-error.log");
            string text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + Environment.NewLine
                + title + Environment.NewLine
                + ex + Environment.NewLine
                + new string('-', 80) + Environment.NewLine;

            try
            {
                File.AppendAllText(logPath, text);
            }
            catch
            {
                // Ignore logging errors.
            }

            MessageBox.Show(
                "La aplicacion encontro un error al iniciar.\n\n"
                + ex.Message
                + "\n\nSe genero el archivo startup-error.log en:\n"
                + logPath,
                "Error de inicio",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }
}
