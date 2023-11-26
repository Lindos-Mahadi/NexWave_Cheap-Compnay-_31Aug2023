<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RA51.aspx.cs" Inherits="comdeeds.RA51" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Comdeeds</title>
      <link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/css/bootstrap.min.css">
  <script src="https://ajax.googleapis.com/ajax/libs/jquery/3.1.1/jquery.min.js"></script>
  <script src="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/js/bootstrap.min.js"></script>
</head>
<body>
    <form id="form1" runat="server">
     <div class="col-md-10 col-md-offset-1">
    <div class="col-md-12">
      <h1>EDGE RA55 PRINT CERTIFICATE</h1>
    </div>
  <div class="col-md-8" >
  <div class="panel panel-info">
  <div class="panel-heading">Company Details</div>
  <div class="panel-body">
      <asp:HiddenField ID="hdncompanyid" runat="server" Value="1193" />
      Company Name
      <asp:TextBox ID="TextBox1" runat="server" CssClass="form-control"></asp:TextBox><br />
      ACN
       <asp:TextBox ID="TextBox2" runat="server" CssClass="form-control"></asp:TextBox>
      <br />
          <asp:Button ID="btnprint" runat="server" Text="VIEW CERTIFICATE (RA55)" 
          CssClass="btn btn-info btn-block" onclick="btnprint_Click"  />
             <asp:Button ID="btnreq" runat="server" Text="REQ" 
          CssClass="btn btn-info btn-block" onclick="btnreq_Click"   />
  </div>
  </div>
       
    </div>
  
    <div class="col-md-7">
     <div class="panel panel-success">
  <div class="panel-heading">Response</div>
  <div class="panel-body">
  <asp:Label ID="lblmsg" runat="server" style="FONT-family: verdana;"></asp:Label>
  </div>
  </div>



    </div>
    </div>
    </form>
</body>
</html>
