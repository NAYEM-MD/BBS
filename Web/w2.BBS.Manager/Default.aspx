<%@ Page Title="" Language="C#" MasterPageFile="~/Form/Common/Default.master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="w2.BBS.Manager.Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
	<h1>Hello, World!</h1>
	<p><%: this.Message %></p>
</asp:Content>
