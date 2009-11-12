<%@ Control Language="C#" AutoEventWireup="true" CodeFile="reportpost.ascx.cs" Inherits="YAF.Pages.reportpost" %>
<YAF:PageLinks runat="server" ID="PageLinks" />
	<table class="content" width="100%" cellspacing="1" cellpadding="0">
		<tr>
			<td class="header1" colspan="2">
				<YAF:LocalizedLabel ID="ReportPostLabel" runat="server" LocalizedTag="HEADER" /><asp:HiddenField ID="MessageIDH" runat="server" />
			</td>			
		</tr> 		  
		<tr>
			<td class="postheader" style="width: 100px" valign="top">
				<YAF:LocalizedLabel ID="EnterReportTextLabel" runat="server" LocalizedTag="ENTER_TEXT" />
			</td>
				<td id="EditorLine" class="post" runat="server">
			<!-- editor goes here -->
		         </td>				
		</tr>
		<tr class="footer1">
			<td colspan="2" align="center">
			<YAF:ThemeButton ID="btnReport" runat="server" CssClass="yafcssbigbutton rightItem" TextLocalizedTag="SEND"
				OnClick="btnReport_Click" />
			<YAF:ThemeButton ID="btnCancel" runat="server" CssClass="yafcssbigbutton rightItem" TextLocalizedTag="CANCEL"
				OnClick="btnCancel_Click" />				
			</td>			
		</tr>	
	</table>

