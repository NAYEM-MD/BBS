<%@ Page Language="C#" MasterPageFile="~/Form/Common/Default.master" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="w2.BBS.Manager.Login" %>

<asp:Content ID="cHead" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="cBody" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
	<asp:Literal ID="litTitle" runat="server" />
	<asp:Literal ID="litError" runat="server" />

	<div class="field">
		<asp:Literal ID="litLblLoginId" runat="server" />
		<asp:TextBox ID="tbLoginId" runat="server" />
	</div>
	<div class="field">
		<asp:Literal ID="litLblPassword" runat="server" />
		<asp:TextBox ID="tbPassword" runat="server" TextMode="Password" />
	</div>
	<asp:LinkButton ID="lbLogin" runat="server" OnClick="lbLogin_OnClick">ログイン</asp:LinkButton>
</asp:Content>
