<%@ Page Language="C#" MasterPageFile="~/Form/Common/Default.master" AutoEventWireup="true" CodeBehind="UserEdit.aspx.cs" Inherits="w2.BBS.Manager.UserEdit" %>
<%-- // (c) 2026 W2 Co.,Ltd. --%>

<asp:Content ID="cHead" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="cBody" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
	<h1>ユーザー編集</h1>

	<asp:Panel ID="pnlMsg" runat="server" Visible="false">
		<p><asp:Literal ID="lMsg" runat="server" /></p>
	</asp:Panel>

	<div class="field">
		<span class="label">ログインID</span>
		<asp:Literal ID="lLoginId" runat="server" />
	</div>
	<div class="field">
		<span class="label">ユーザー名</span>
		<asp:TextBox ID="tbUserName" runat="server" />
	</div>
	<div class="field">
		<span class="label">新パスワード</span>（空なら変更しない）
		<asp:TextBox ID="tbPassword" runat="server" TextMode="Password" />
	</div>

	<asp:LinkButton ID="lbSave" runat="server" OnClick="lbSave_OnClick">保存</asp:LinkButton>
	&nbsp;
	<asp:LinkButton ID="lbDeleteUser" runat="server" CausesValidation="false" OnClick="lbDeleteUser_OnClick"
		OnClientClick="return confirm('このユーザーを削除しますか？（投稿・返信も論理削除）');">ユーザー削除</asp:LinkButton>
	&nbsp;
	<asp:LinkButton ID="lbBack" runat="server" CausesValidation="false" OnClick="lbBack_OnClick">一覧へ</asp:LinkButton>
</asp:Content>
