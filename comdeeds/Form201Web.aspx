<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Form201Web.aspx.cs" Inherits="comdeeds.Form201Web" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>ASIC COMMUNICATION RA53</title>
    <meta charset="utf-8">
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/css/bootstrap.min.css">
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/3.1.1/jquery.min.js"></script>
    <script src="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/js/bootstrap.min.js"></script>
</head>
<body>
    <form id="form1" runat="server">
        <br />
   
        <div class="col-md-10 col-md-offset-1">
            <div class="col-md-12">
                <h1>ASIC COMMUNICATION FORM-201</h1>
            </div>
            <div class="col-md-3">
                <div class="panel panel-primary">
                    <div class="panel-heading">BASIC CALL</div>
                    <div class="panel-body">
                        <asp:HiddenField ID="hdncompanyid" runat="server" Value="1193" />
                        Email-ID
      <asp:TextBox ID="txtemailid" CssClass="form-control input-sm" runat="server">deepak.dubey@gmail.com</asp:TextBox>
                        Company-ID
      <asp:TextBox ID="txtcompanyid" CssClass="form-control input-sm" runat="server">152</asp:TextBox>
                        <asp:Button ID="btnget201database" runat="server" Text="Get 201 DataBase"
                            CssClass="btn btn-danger btn-sm btn-block" OnClick="btnget201database_Click" /><br />
                        <asp:Button ID="btnform201" runat="server" Text="FORM201 FILE" Enabled="true"
                            CssClass="btn btn-warning btn-block" OnClick="btnform201_Click" />
                        <asp:Button ID="btnreq" runat="server" Text="REQ" Enabled="true"
                            CssClass="btn btn-info btn-block" OnClick="btnreq_Click" />
                    </div>
                </div>

            </div>

            <div class="col-md-9">
                <div class="panel panel-success">
                    <div class="panel-heading">Response</div>
                    <div class="panel-body">
                        <asp:Label ID="lblmsg" runat="server" Style="font-family: verdana;"></asp:Label>
                        <asp:Label ID="lblasicerr_" runat="server" Text=""></asp:Label>
                    </div>
                </div>



            </div>
        </div>
    </form>
</body>
</html>
