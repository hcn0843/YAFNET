using System;
using System.IO;
using System.Reflection;
using System.Web.UI;
using System.Text.RegularExpressions;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace yaf.editor
{

	/// <summary>
	/// Summary description for ForumEditorBase.
	/// </summary>
	public class ForumEditor : Control
	{
		protected	string m_baseDir = string.Empty;

		public new string ResolveUrl(string relativeUrl)
		{
			if(m_baseDir!=string.Empty)
				return m_baseDir + relativeUrl;
			else
				return base.ResolveUrl(relativeUrl);
		}

		protected virtual string Replace(string txt,string match,string replacement) 
		{
			RegexOptions options = RegexOptions.IgnoreCase | RegexOptions.Multiline;
			while (Regex.IsMatch(txt,match,options)) txt = Regex.Replace(txt,match,replacement,options);
			return txt;
		}

		#region Virtual Properties
		public virtual string Text
		{
			get	{	return string.Empty; }
			set	{	; }
		}

		public virtual string BaseDir
		{
			set
			{
				m_baseDir = value;
				if (!m_baseDir.EndsWith("/"))
					m_baseDir += "/";
			}
		}
		public virtual string StyleSheet
		{
			set { ;	}
		}
		public virtual bool UsesHTML
		{
			get	{ return false; }		
		}
		public virtual bool UsesBBCode
		{
			get { return false; }
		}
		#endregion
	}


	public class TextEditor : ForumEditor
	{
		protected TextBox	m_textCtl;

		protected override void OnInit(EventArgs e)
		{			
			Load += new EventHandler(Editor_Load);
			m_textCtl = new TextBox();
			m_textCtl.ID = "edit";
			m_textCtl.Attributes.Add("style","height:100%;width:100%;");
			m_textCtl.TextMode = TextBoxMode.MultiLine;
			Controls.Add(m_textCtl);
			base.OnInit(e);
		}

		protected virtual void Editor_Load(object sender,EventArgs e)
		{
			if (this.Visible)
			{
				Page.RegisterClientScriptBlock("insertsmiley",
					"<script language='javascript'>\n"+
					"function insertsmiley(code) {\n"+
					"	" + SafeID + ".InsertSmiley(code);\n"+
					"}\n"+
					"</script>\n");
			}
		}

		public override string Text
		{
			get
			{
				return m_textCtl.Text;
			}
			set
			{
				m_textCtl.Text = value;
			}
		}

		protected string SafeID
		{
			get	{	return m_textCtl.ClientID.Replace("$","_");	}
		}

		public override bool UsesHTML
		{
			get	{ return false; }		
		}
		public override bool UsesBBCode
		{
			get { return false; }
		}
	}


	public class BBCodeEditor : TextEditor
	{
		private void RenderButton(HtmlTextWriter writer,string id,string cmd,string title,string image)
		{
			//writer.WriteLine("		<td><img id='{1}_{4}' onload='Button_Load(this)' src='{0}' width='21' height='20' alt='{2}' title='{2}' onclick=\"{1}.{3}\"></td><td>&nbsp;</td>",ResolveUrl(image),SafeID,title,cmd,id);
			writer.WriteLine("<img id='{1}_{4}' onload='Button_Load(this)' src='{0}' width='21' height='20' alt='{2}' title='{2}' onclick=\"setStyle('{4}','')\">",ResolveUrl(image),SafeID,title,cmd,id);
		}

		protected override void Render(HtmlTextWriter writer)
		{
			writer.WriteLine("");
			writer.WriteLine("<script language='javascript'>");
			writer.WriteLine("var {0}=new yafEditor('{0}');",SafeID);
			writer.WriteLine("function setStyle(style,option) {");
			writer.WriteLine("	{0}.FormatText(style,option);",SafeID);
			writer.WriteLine("}");
			writer.WriteLine("</script>");

			writer.WriteLine("<table border='0' cellpadding='0' cellspacing='2' width='100%' height='300'>");
			writer.WriteLine("<tr><td valign='top'>");
			writer.WriteLine("	<table border='0' cellpadding='1' cellspacing='2'>");
			writer.WriteLine("	<tr><td valign='middle'>");

			RenderButton(writer,"bold","FormatText('bold','')","Bold","yafEditor/bold.gif");
			RenderButton(writer,"italic","FormatText('italic','')","Italic","yafEditor/italic.gif");
			RenderButton(writer,"underline","FormatText('underline','')","Underline","yafEditor/underline.gif");

			writer.WriteLine("&nbsp;");

			RenderButton(writer,"quote","FormatText('quote','')","Quote","yafEditor/quote.gif");
			RenderButton(writer,"code","FormatText('code','')","Code","yafEditor/code.gif");
			RenderButton(writer,"img","FormatText('img','')","Image","yafEditor/image.gif");
			RenderButton(writer,"createlink","FormatText('createlink','')","Create Link","yafEditor/link.gif");

			writer.WriteLine("&nbsp;");

			RenderButton(writer,"justifyleft","FormatText('justifyleft','')","Left","yafEditor/justifyleft.gif");
			RenderButton(writer,"justifycenter","FormatText('justifycenter','')","Center","yafEditor/justifycenter.gif");
			RenderButton(writer,"justifyright","FormatText('justifyright','')","Right","yafEditor/justifyright.gif");

			writer.WriteLine("	</td></tr>");
			writer.WriteLine("	<tr><td valign='middle'>");

			writer.WriteLine("Font color:");
			writer.WriteLine("<select onchange=\"if(this.value!='') setStyle('color',this.value); this.value=''\">",SafeID);
			writer.WriteLine("<option value=\"\">Default</option>");
			
			string[] Colors = {"Dark Red","Red","Orange","Brown","Yellow","Green","Olive","Cyan","Blue","Dark Blue","Indigo","Violet","White","Black"};
			foreach(string color in Colors)
			{
				string tValue = color.Replace(" ","").ToLower();
				writer.WriteLine(string.Format("<option style=\"color:{0}\" value=\"{0}\">{1}</option>",tValue,color));
			}

			writer.WriteLine("</select>");

			writer.WriteLine("Font size:");
			writer.WriteLine("<select onchange=\"if(this.value!='') setStyle('fontsize',this.value); this.value=''\">",SafeID);
			writer.WriteLine("<option value=\"1\">1</option>");
			writer.WriteLine("<option value=\"2\">2</option>");
			writer.WriteLine("<option value=\"3\">3</option>");
			writer.WriteLine("<option value=\"4\">4</option>");
			writer.WriteLine("<option selected=\"selected\" value=\"5\">Default</option>");
			writer.WriteLine("<option value=\"6\">6</option>");
			writer.WriteLine("<option value=\"7\">7</option>");
			writer.WriteLine("<option value=\"8\">8</option>");
			writer.WriteLine("<option value=\"9\">9</option>");
			writer.WriteLine("</select>");

			writer.WriteLine("	</td></tr>");
			writer.WriteLine("	</table>");
			writer.WriteLine("</td></tr><tr><td height='99%'>");
			base.Render(writer);
			writer.WriteLine("</td></tr></table>");
		}

		protected override void OnInit(EventArgs e)
		{
			
			base.OnInit(e);
		}

		protected override void Editor_Load(object sender,EventArgs e)
		{
			if(this.Visible)
			{
				Page.RegisterClientScriptBlock("richeditstyles",
					"<style>\n"+
					".ButtonOut\n"+
					"{\n"+
					"	filter: alpha(opacity=70);\n"+
					"	border: #7F9DB9 1px solid;\n"+
					"}\n"+
					".ButtonOver\n"+
					"{\n"+
					"	background-color: #FFE1AC;\n"+
					"	border: #FFAD55 1px solid;\n"+
					"}\n"+
					".ButtonChecked\n"+
					"{\n"+
					"	background-color: #FFCB7E;\n"+
					"	border: #FFAD55 1px solid;\n"+
					"}\n"+
					".ButtonOff\n"+
					"{\n"+
					"	filter: gray() alpha(opacity=30);\n"+
					"	background-color: #C0C0C0;\n"+
					"	border: #7F9DB9 1px solid;\n"+
					"}\n"+
					"</style>\n");
				
				Page.RegisterClientScriptBlock("yafeditorjs",string.Format("<script language='javascript' src='{0}'></script>",ResolveUrl("yafEditor/yafEditor.js")));

				Page.RegisterClientScriptBlock("insertsmiley",
					"<script language='javascript'>\n"+
					"function insertsmiley(code) {\n"+
					"	" + SafeID + ".InsertSmiley(code);\n"+
					"}\n"+
					"</script>\n");
			}
		}

		#region Properties

		public override bool UsesBBCode
		{
			get { return true; }
		}
		#endregion
	}


	public class RichClassEditor : ForumEditor
	{
		protected bool bInit;
		protected Type typEditor;
		protected System.Web.UI.Control objEditor;
		protected Assembly cBin;

		public RichClassEditor(string BinFile,string ClassName)
		{
			bInit = false;

			BinFile = System.Web.HttpContext.Current.Server.MapPath(BinFile);
	
			try
			{
				cBin = Assembly.LoadFrom(BinFile);
				// get all the types in the loaded assembly
				Type[] types = cBin.GetTypes();

				foreach (Type typ in types)
				{
					// dynamically create or activate(if exist) object
					if (typ.FullName == ClassName)
					{
						typEditor = typ;
						// create this object
						objEditor = (System.Web.UI.Control)Activator.CreateInstance(typ);
						bInit = true;
						break;
					}
				}
			}
			catch(Exception e)
			{
#if DEBUG
				throw new Exception(e.Message);
#endif
			}
		}

		#region Properties

		protected string SafeID
		{
			get
			{
				if (bInit)
				{
					return objEditor.ClientID.Replace("$","_");
				}
				return string.Empty;															 
			}
		}

		public override bool UsesHTML
		{
			get	{ return true; }		
		}
		public override bool UsesBBCode
		{
			get { return false; }
		}
		#endregion

	}


	public class FCKEditor : RichClassEditor
	{
		public FCKEditor() : base("bin\\FredCK.FCKeditorV2.dll","FredCK.FCKeditorV2.FCKeditor")
		{
			
		}

		protected override void OnInit(EventArgs e)
		{			
			if (bInit)
			{
				Load += new EventHandler(Editor_Load);
				PropertyInfo pInfo = typEditor.GetProperty("ID");
				pInfo.SetValue(objEditor,"edit",null);
				Controls.Add(objEditor);
			}
			base.OnInit(e);
		}

		protected virtual void Editor_Load(object sender,EventArgs e)
		{
			if (bInit && objEditor.Visible)
			{
				PropertyInfo pInfo;
				pInfo = typEditor.GetProperty("BasePath");
				pInfo.SetValue(objEditor,ResolveUrl("FCKEditor/"),null);

				Page.RegisterClientScriptBlock("fckeditorjs",string.Format("<script language='javascript' src='{0}'></script>",ResolveUrl("FCKEditor/FCKEditor.js")));
			}
		}

		#region Properties
		public override string Text
		{
			get
			{
				if (bInit)
				{
					PropertyInfo pInfo = typEditor.GetProperty("Value");
					return Convert.ToString(pInfo.GetValue(objEditor,null));
				}
				else return string.Empty;
			}
			set
			{
				if (bInit)
				{
					PropertyInfo pInfo = typEditor.GetProperty("Value");
					pInfo.SetValue(objEditor,value,null);
				}
			}
		}
		#endregion
	}


	public class FreeTextBoxEditor : RichClassEditor
	{
		public FreeTextBoxEditor() : base("bin\\FreeTextBox.dll","FreeTextBoxControls.FreeTextBox")
		{
			
		}

		protected override void OnInit(EventArgs e)
		{			
			if (bInit)
			{
				Load += new EventHandler(Editor_Load);
				PropertyInfo pInfo = typEditor.GetProperty("ID");
				pInfo.SetValue(objEditor,"edit",null);
				Controls.Add(objEditor);
			}
			base.OnInit(e);
		}

		protected virtual void Editor_Load(object sender,EventArgs e)
		{
			if (bInit && objEditor.Visible)
			{
				PropertyInfo pInfo;
				pInfo = typEditor.GetProperty("SupportFolder");
				pInfo.SetValue(objEditor,ResolveUrl("FreeTextBox/"),null);
				pInfo = typEditor.GetProperty("Width");
				pInfo.SetValue(objEditor,Unit.Percentage(100),null);

				// toolbars

/*			Toolbars.Clear();
 			
				Toolbar toolbar1 = new Toolbar();
				toolbar1.Items.Add(new ParagraphMenu());
				toolbar1.Items.Add(new FontSizesMenu());
				toolbar1.Items.Add(new FontForeColorsMenu());
				this.Toolbars.Add(toolbar1);

				Toolbar toolbar2 = new Toolbar();
				toolbar2.Items.Add(new Bold());
				toolbar2.Items.Add(new Italic());
				toolbar2.Items.Add(new Underline());
				toolbar2.Items.Add(new ToolbarSeparator());
				toolbar2.Items.Add(new Cut());
				toolbar2.Items.Add(new Copy());
				toolbar2.Items.Add(new Paste());
				toolbar2.Items.Add(new Delete());
				toolbar2.Items.Add(new ToolbarSeparator());
				toolbar2.Items.Add(new JustifyLeft());
				toolbar2.Items.Add(new JustifyCenter());
				toolbar2.Items.Add(new JustifyRight());
				toolbar2.Items.Add(new JustifyFull());
				toolbar2.Items.Add(new ToolbarSeparator());
				toolbar2.Items.Add(new BulletedList());
				toolbar2.Items.Add(new NumberedList());
				toolbar2.Items.Add(new ToolbarSeparator());
				toolbar2.Items.Add(new Outdent());
				toolbar2.Items.Add(new Indent());
				toolbar2.Items.Add(new ToolbarSeparator());
				toolbar2.Items.Add(new CreateLink());
				toolbar2.Items.Add(new Unlink());
				//toolbar2.Items.Add(new InsertImage());
				this.Toolbars.Add(toolbar2);
*/				

				Page.RegisterClientScriptBlock("insertsmiley",
					"<script language='javascript'>\n"+
					"function insertsmiley(code){"+
					"FTB_InsertText('" + SafeID + "',code);"+
					"}\n"+
					"</script>\n");
			}
		}

		#region Properties
		public override string Text
		{
			get
			{
				if (bInit)
				{
					PropertyInfo pInfo = typEditor.GetProperty("Text");
					return Convert.ToString(pInfo.GetValue(objEditor,null));
				}
				else return string.Empty;
			}
			set
			{
				if (bInit)
				{
					PropertyInfo pInfo = typEditor.GetProperty("Text");
					pInfo.SetValue(objEditor,value,null);
				}
			}
		}
		#endregion
	}

}
