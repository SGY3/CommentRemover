﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Index.aspx.cs" Inherits="CommentRemover.Index" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <title>Commnent Remover</title>
    <link href="~\lib\bootstrap\css\bootstrap.min.css" rel="stylesheet" />
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="btnRemoveCommnet" EventName="OnClick" />
            </Triggers>
            <ContentTemplate>
                <div class="container border shadow rounded-2 my-3 p-3">
                    <div class="row mb-2">
                        <div class="col-md-2">
                            <asp:Label ID="lblSelectType" runat="server" Text="Select Type" CssClass="form-label"></asp:Label>
                        </div>
                        <div class="col-md-10">
                            <asp:RadioButtonList ID="radType" runat="server" RepeatDirection="Horizontal" AutoPostBack="true" OnSelectedIndexChanged="radType_SelectedIndexChanged">
                                <asp:ListItem Selected="True">Text</asp:ListItem>
                                <asp:ListItem>File</asp:ListItem>
                            </asp:RadioButtonList>
                        </div>
                    </div>
                    <div class="mb-2" id="divText" runat="server">
                        <div class="row mb-2">
                            <div class="col-md-2">
                                <asp:Label ID="lblText" runat="server" Text="Enter Text" CssClass="form-label"></asp:Label>
                            </div>
                            <div class="col-md-10">
                                <asp:TextBox ID="txtFileName" runat="server" CssClass="form-control" placeholder="Enter file name (optional)"></asp:TextBox>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-12">
                                <asp:TextBox ID="txtText" runat="server" Rows="10" TextMode="MultiLine" CssClass="form-control" placeholder="Paste your text here..."></asp:TextBox>
                            </div>
                        </div>
                    </div>
                    <div class="row mb-2" id="divFile" runat="server" visible="false">
                        <div class="col-md-2">
                            <asp:Label ID="lblSelect" runat="server" Text="Select File"></asp:Label>
                        </div>
                        <div class="col-md-10">
                            <asp:FileUpload ID="fuFile" runat="server" CssClass="form-control" AllowMultiple="true" accept=".txt,.sql" />
                        </div>
                    </div>
                    <div class="d-flex justify-content-center align-items-center">
                        <asp:Button ID="btnRemoveComment" runat="server" Text="Remove Comments" CssClass="btn btn-success btn-sm" OnClick="btnRemoveComment_Click" />
                    </div>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </form>
    <script src="~\lib\bootstrap\js\bootstrap.min.js"></script>
</body>
</html>