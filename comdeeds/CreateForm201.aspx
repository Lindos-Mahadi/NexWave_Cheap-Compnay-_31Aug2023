<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CreateForm201.aspx.cs" Inherits="comdeeds.CreateForm201" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
          <asp:HiddenField ID="hdncompanyid" runat="server" />
          <asp:HiddenField ID="hdncompanyname" runat="server" />
          <asp:HiddenField ID="hdnacn" runat="server" />
          <asp:HiddenField ID="hdnabn" runat="server" />
          <asp:HiddenField ID="hdnAddress" runat="server" />
          <asp:HiddenField ID="hdnemail" runat="server" />
    <br /><br />
    <asp:Button ID="Button1" runat="server" Text="Unable to create Form201, Please check your submitted Data" onclick="Button1_Click" Enabled="false" />
    <br />
    <br />
    </div>
    </form>
</body>
</html>
