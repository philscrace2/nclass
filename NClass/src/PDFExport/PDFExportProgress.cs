﻿using System;
using System.Threading;
using System.Windows.Forms;

namespace PDFExport
{
  /// <summary>
  /// A "please wait" form for the PDFExport.
  /// </summary>
  public partial class PDFExportProgress : Form
  {
    /// <summary>
    /// The x-location where the entity starts to move.
    /// </summary>
    private const int ENTITY_START = 42;

    /// <summary>
    /// The x-location where the entity end its move.
    /// </summary>
    private const int ENTITY_END = 232;

    /// <summary>
    /// The current index of the entity which is displayed.
    /// </summary>
    private int currentImageIndex;

    /// <summary>
    /// Initializes a new PDFExportProgress form.
    /// </summary>
    private PDFExportProgress()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Called when the timer ticks. So this method is called periodically
    /// and updates the animation.
    /// </summary>
    /// <param name="sender">The caller.</param>
    /// <param name="e">Additional information.</param>
    private void timer_Tick(object sender, EventArgs e)
    {
      if(pictureBoxEntity.Left == ENTITY_END)
      {
        //Next run
        pictureBoxEntity.Left = ENTITY_START;
        currentImageIndex = (currentImageIndex + 1) % imageListEntities.Images.Count;
        pictureBoxEntity.Image = imageListEntities.Images[currentImageIndex];
      }
      pictureBoxEntity.Left += 2;
    }

    /// <summary>
    /// Called when the form loads. Initializes it.
    /// </summary>
    /// <param name="sender">The caller.</param>
    /// <param name="e">Additional information.</param>
    private void PDFExportProgress_Load(object sender, EventArgs e)
    {
      timer.Enabled = true;
      pictureBoxEntity.Image = imageListEntities.Images[currentImageIndex];
    }

    /// <summary>
    /// A thread which shows the form itself.
    /// </summary>
    private static Thread showThread;

    /// <summary>
    /// Shows the form asynchonously. To close it, call <see cref="CloseAsync"/>.
    /// </summary>
    /// <param name="parent">The parent form.</param>
    public static void ShowAsync(Form parent)
    {
      showThread = new Thread(Run)
                     {
                       IsBackground = true, 
                       Name = "PDFExporterProgressDialogShowThread"
                     };
      showThread.Start(parent);
    }

    /// <summary>
    /// Closes an opened PDFExporterProgress form if it is opened.
    /// </summary>
    public static void CloseAsync()
    {
      if(showThread != null && showThread.IsAlive)
      {
        showThread.Abort();
        showThread.Join();
      }
    }

    /// <summary>
    /// Run-method of the display thread. Shows the form and waits
    /// for the end.
    /// </summary>
    /// <param name="parentForm">The parent form.</param>
    private static void Run(Object parentForm)
    {
      PDFExportProgress progressForm = new PDFExportProgress();
      progressForm.Show((Form)parentForm);

      try
      {
        while(true)
        {
          Thread.Sleep(5);
          Application.DoEvents();
        }
      }
      catch(ThreadAbortException)
      {
        //We are asked to close the window
        progressForm.Close();
        progressForm.Dispose();
      }
    }
  }
}