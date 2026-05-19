<%@ Page Language="C#" MasterPageFile="~/Form/Common/Default.master" AutoEventWireup="true" CodeBehind="UserEdit.aspx.cs" Inherits="w2.BBS.Manager.UserEdit" %>

<asp:Content ID="cHead" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="cBody" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
	<asp:Literal ID="litTitle" runat="server" />
	<asp:Literal ID="litMsg" runat="server" />

	<div class="field">
		<asp:Literal ID="litLblLoginId" runat="server" />
		<asp:Literal ID="litLoginId" runat="server" />
	</div>
	<div class="field">
		<asp:Literal ID="litLblUserName" runat="server" />
		<asp:TextBox ID="tbUserName" runat="server" />
	</div>
	<div class="field">
		<asp:Literal ID="litLblPassword" runat="server" />
		<asp:TextBox ID="tbPassword" runat="server" TextMode="Password" />
	</div>

	<asp:LinkButton ID="lbSave" runat="server" OnClick="lbSave_OnClick">保存</asp:LinkButton>
	&nbsp;
	<asp:LinkButton ID="lbDeleteUser" runat="server" CausesValidation="false" OnClick="lbDeleteUser_OnClick"
		OnClientClick="return confirm('このユーザーを削除しますか？（投稿・返信も論理削除）');">ユーザー削除</asp:LinkButton>
	&nbsp;
	<asp:LinkButton ID="lbBack" runat="server" CausesValidation="false" OnClick="lbBack_OnClick">一覧へ</asp:LinkButton>
</asp:Content>
