<%@ Page Language="C#" MasterPageFile="~/Form/Common/Default.master" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="w2.BBS.Manager.Login" %>
<%-- // (c) 2026 W2 Co.,Ltd. --%>

<asp:Content ID="cHead" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="cBody" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
	<h1>管理者ログイン</h1>

	<asp:Panel ID="pnlError" runat="server" CssClass="error" Visible="false">
		<p><asp:Literal ID="lError" runat="server" /></p>
	</asp:Panel>

	<div class="field">
		<span class="label">ログインID</span>
		<asp:TextBox ID="tbLoginId" runat="server" />
	</div>
	<div class="field">
		<span class="label">パスワード</span>
		<asp:TextBox ID="tbPassword" runat="server" TextMode="Password" />
	</div>
	<asp:LinkButton ID="lbLogin" runat="server" OnClick="lbLogin_OnClick">ログイン</asp:LinkButton>
</asp:Content>
