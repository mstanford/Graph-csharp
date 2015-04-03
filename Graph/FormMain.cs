// ------------------------------------------------------------------------
// 
// This is free and unencumbered software released into the public domain.
// 
// Anyone is free to copy, modify, publish, use, compile, sell, or
// distribute this software, either in source code form or as a compiled
// binary, for any purpose, commercial or non-commercial, and by any
// means.
// 
// In jurisdictions that recognize copyright laws, the author or authors
// of this software dedicate any and all copyright interest in the
// software to the public domain. We make this dedication for the benefit
// of the public at large and to the detriment of our heirs and
// successors. We intend this dedication to be an overt act of
// relinquishment in perpetuity of all present and future rights to this
// software under copyright law.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS BE LIABLE FOR ANY CLAIM, DAMAGES OR
// OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
// ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
// 
// For more information, please refer to <http://unlicense.org/>
// 
// ------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Graph
{
	public partial class FormMain : Form
	{
		private readonly Scripting.Evaluator _evaluator;
		private readonly object _evaluationLock = new object();
		private bool _evaluationFlag = false;
		private readonly string _path;
		private readonly Scripting.Frame _frame = new Scripting.Frame();
		private readonly Widgets.Style _style = new Widgets.Style();
		private List<Scripting.Expression> _expressions;
		private Widgets.Widget _widget;
		private bool _isMouseDown;
		private System.Drawing.Point _mouseLocation;

		public FormMain()
		{
			InitializeComponent();
		}

		public FormMain(string[] args)
		{
			if (args.Length != 1)
				throw new System.Exception();
			_path = args[0];

			InitializeComponent();

			//DoubleBuffered = true;
			SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
			ResizeRedraw = true;

			KeyPreview = true;

			Text = System.IO.Path.GetFileNameWithoutExtension(_path);

			System.IO.StreamReader definitionReader = new System.IO.StreamReader(typeof(Scripting.Evaluator).Assembly.GetManifestResourceStream(typeof(Scripting.Evaluator).Namespace + ".Evaluator.def"));
			_evaluator = new Scripting.Evaluator(definitionReader);
			definitionReader.Close();

			System.Threading.Thread thread = new System.Threading.Thread(new System.Threading.ThreadStart(EvaluationThread));
			thread.IsBackground = true;
			thread.Start();
		}

		private void Evaluate()
		{
			_widget = null;

			_evaluationFlag = true;

			lock (_evaluationLock)
			{
				System.Threading.Monitor.Pulse(_evaluationLock);
			}
		}

		private void EvaluationThread()
		{
			while (true)
			{
				lock (_evaluationLock)
				{
					System.Threading.Monitor.Wait(_evaluationLock);
				}

				while (_evaluationFlag)
				{
					try
					{
						_evaluationFlag = false;

						List<Widgets.Widget> widgets = new List<Widgets.Widget>();
						_evaluator.EvaluateScript(_expressions, _frame, widgets);

						Widgets.Column column = new Widgets.Column(widgets.ToArray());
						column.Initialize(_frame, _style);
						System.Drawing.RectangleF clientRectangle = ClientRectangle;
						column.Layout(new System.Drawing.RectangleF(clientRectangle.X + 2, clientRectangle.Y + 2, clientRectangle.Width - 4, clientRectangle.Height - 4));
						if (_isMouseDown)
							column.MouseDown(_mouseLocation);
						_widget = column;


						System.Drawing.Size size = Size;
						bool modifiedSize = false;
						if (_frame.Values.ContainsKey("$$width"))
						{
							size.Width = Graph.Convert.ToInt32(_frame.Values["$$width"]);
							modifiedSize = true;
						}
						if (_frame.Values.ContainsKey("$$height"))
						{
							size.Height = Graph.Convert.ToInt32(_frame.Values["$$height"]);
							modifiedSize = true;
						}
						if (modifiedSize)
							Invoke(new Action<System.Drawing.Size>(SetSize), size);


						Invoke(new Action(Invalidate));
					}
					catch (System.Exception exception)
					{
						System.Threading.Thread.Sleep(500);
					}
				}
			}
		}

		private void SetSize(System.Drawing.Size size)
		{
			Size = size;
		}

		protected override void OnShown(EventArgs e)
		{
			base.OnShown(e);

			if (System.IO.File.Exists(_path + ".usr"))
			{
				try
				{
					//TODO load from user setting file.
					////TODO use the native tongue.
					//// Load settings
					//if (System.IO.File.Exists(_path + ".usrx"))
					//{
					//    try
					//    {
					//        System.Xml.XmlDocument xmlDocument = new System.Xml.XmlDocument();
					//        xmlDocument.Load(_path + ".usrx");

					//        System.Xml.XmlElement userSettingsElement = xmlDocument.DocumentElement;
					//        Left = Convert.ToInt32(userSettingsElement.Attributes["left"].Value);
					//        Top = Convert.ToInt32(userSettingsElement.Attributes["top"].Value);
					//        Width = Convert.ToInt32(userSettingsElement.Attributes["width"].Value);
					//        Height = Convert.ToInt32(userSettingsElement.Attributes["height"].Value);

					//        System.Xml.XmlElement controlsElement = (System.Xml.XmlElement)userSettingsElement.GetElementsByTagName("Controls")[0];
					//        if (controlsElement != null)
					//        {
					//            foreach (System.Xml.XmlNode xmlNode in controlsElement.ChildNodes)
					//            {
					//                if (xmlNode is System.Xml.XmlElement)
					//                {
					//                    System.Xml.XmlElement parameterElement = (System.Xml.XmlElement)xmlNode;
					//                    string name = parameterElement.Attributes["name"].Value;
					//                    string value = parameterElement.Attributes["value"].Value;
					//                    frame = new Wind.Language.Frame(frame, name, value);
					//                }
					//            }
					//        }
					//    }
					//    catch (System.Exception)
					//    {
					//    }
					//}
				}
				catch (System.Exception)
				{
				}
			}

			_frame.Updated += FrameUpdated;

			FileRefresh();
		}

		protected override void OnFormClosed(FormClosedEventArgs e)
		{
			base.OnFormClosed(e);

			//TODO walk the Widget.

			//private void UpdateFrame(Widgets.Widget widget, Dictionary<string, object> frame)
			//{
			//    if (widget.Name != null)
			//        frame[widget.Name] = widget.Value;

			//    if (widget.Widgets != null)
			//        foreach (Widgets.Widget widget2 in widget.Widgets)
			//            UpdateFrame(widget2, frame);
			//}

			/*
			<UserSettings left="214" top="59" width="1200" height="892">
			  <Parameters>
				<Parameter name="symbol" value="USDCHF" />
				<Parameter name="lookback" value="1" />
				<Parameter name="adjustment" value="1" />
				<Parameter name="week" value="20060730" />
				<Parameter name="trendStepSize" value="2" />
			  </Parameters>
			</UserSettings>
			*/

			////TODO use the native tongue.
			//// Save settings
			//System.IO.TextWriter writer = System.IO.File.CreateText(_path + ".usrx");
			//writer.WriteLine("<UserSettings left=\"" + Left + "\" top=\"" + Top + "\" width=\"" + Width + "\" height=\"" + Height + "\">");
			//writer.WriteLine("  <Controls>");
			//Wind.Language.Frame frame = _canvas.Frame;
			//if (frame != null)
			//    foreach (KeyValuePair<string, Wind.Graphics.Controls.Control> kvp in frame.Controls)
			//        writer.WriteLine("    <Control name=\"" + kvp.Value.Name + "\" value=\"" + kvp.Value.Value + "\" />");
			//writer.WriteLine("  </Controls>");
			//writer.WriteLine("</UserSettings>");
			//writer.Flush();
			//writer.Close();
		}

		private void FrameUpdated(object sender, System.EventArgs e)
		{
			Evaluate();
		}

		private void FileRefresh()
		{
			_widget = null;
			_expressions = null;

			if (System.IO.File.Exists(_path))
			{
				using (System.IO.Stream stream = System.IO.File.Open(_path, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.Read))
				{
					System.IO.StreamReader streamReader = new System.IO.StreamReader(stream);
					_expressions = new List<Scripting.Expression>(Scripting.Parser.Parse(streamReader, _evaluator.InfixOperators));
					stream.Close();

					//The expression are reloaded.  Reuse the prior frame.
					Evaluate();
				}
			}
		}

		protected override void OnKeyDown(KeyEventArgs e)
		{
			base.OnKeyDown(e);

			StringBuilder stringBuilder = new StringBuilder();
			if (e.Control) stringBuilder.Append("Ctrl+");
			if (e.Shift) stringBuilder.Append("Shift+");
			if (e.Alt) stringBuilder.Append("Alt+");
			switch (e.KeyCode)
			{
				case Keys.Left:
					stringBuilder.Append("Left");
					break;
				case Keys.Right:
					stringBuilder.Append("Right");
					break;
				case Keys.Home:
					stringBuilder.Append("Home");
					break;
				case Keys.End:
					stringBuilder.Append("End");
					break;
				default:
					stringBuilder.Append(e.KeyCode.ToString());
					break;
			}

			string key = stringBuilder.ToString();
			switch (key)
			{
				case "F5":
					FileRefresh();
					break;
				case "Escape":
					if (WindowState == FormWindowState.Maximized)
					{
						FormBorderStyle = FormBorderStyle.SizableToolWindow;
						WindowState = FormWindowState.Normal;
					}
					break;
				case "F11":
					if (WindowState == FormWindowState.Maximized)
					{
						FormBorderStyle = FormBorderStyle.SizableToolWindow;
						WindowState = FormWindowState.Normal;
					}
					else
					{
						FormBorderStyle = FormBorderStyle.None;
						WindowState = FormWindowState.Maximized;
					}
					break;
				case "F12":
					//TODO show text editor to modify source script.
					//If the user clicks save in the editor refresh and reevaluate.
					//If the user closes the editor, just hide it.
					//If a hidden editor is already open just unhide it so that the user doesn't lose their place.
					break;
				default:
					Widgets.Widget widget = _widget;
					if (widget != null)
						widget.KeyDown(key);
					break;
			}
		}

		protected override void OnClick(EventArgs e)
		{
			base.OnClick(e);

			Widgets.Widget widget = _widget;
			if (widget != null)
				widget.Click(_mouseLocation);
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);

			_isMouseDown = true;
			_mouseLocation = e.Location;

			Widgets.Widget widget = _widget;
			if (widget != null)
				widget.MouseDown(_mouseLocation);
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			base.OnMouseUp(e);

			_isMouseDown = false;
			_mouseLocation = e.Location;

			Widgets.Widget widget = _widget;
			if (widget != null)
				widget.MouseUp(_mouseLocation);
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);

			_mouseLocation = e.Location;

			Widgets.Widget widget = _widget;
			if (widget != null)
				widget.MouseMove(_mouseLocation);
		}

		//protected override void OnMouseWheel(MouseEventArgs e)
		//{
		//    base.OnMouseWheel(e);

		//    Widgets.Widget widget = _widget;
		//    if (widget != null)
		//        widget.MouseWheel(e.Delta);
		//}

		//protected override void OnMouseLeave(EventArgs e)
		//{
		//    base.OnMouseLeave(e);

		//    _isMouseDown = false;

		//    Widgets.Widget widget = _widget;
		//    if (widget != null)
		//        widget.MouseLeave();
		//}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);

			Widgets.Widget widget = _widget;
			if (widget != null)
			{
				System.Drawing.RectangleF clientRectangle = ClientRectangle;
				widget.Render(e.Graphics, new System.Drawing.RectangleF(clientRectangle.X + 2, clientRectangle.Y + 2, clientRectangle.Width - 4, clientRectangle.Height - 4));
			}
		}

	}
}