<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GeneratePDF.aspx.cs" Inherits="comdeeds.GeneratePDF" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/css/bootstrap.min.css">
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/3.1.1/jquery.min.js"></script>
    <script src="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/js/bootstrap.min.js"></script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <h4>Some technical problem while generating the documents.</h4>
          <asp:HiddenField ID="hdncompanyid" runat="server" />
          <asp:HiddenField ID="hdncompanyname" runat="server" />
          <asp:HiddenField ID="hdnacn" runat="server" />
          <asp:HiddenField ID="hdnabn" runat="server" />
          <asp:HiddenField ID="hdnAddress" runat="server" />
          <asp:HiddenField ID="hdnemail" runat="server" />
          <div id="errormsg" runat="server"></div>
    </div>
    </form>
</body>
</html>
