<%@ Page Language="C#" MasterPageFile="~/global.Master" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="gradproj_webapp.Login" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">

    <script src="js/Login.js" type="text/javascript"></script>
    <script src="js/Scripts.js" type="text/javascript"></script>

    <script type="text/javascript">

        $(document).ready(function () {
            initiateLoginPage();

        });

    </script>


</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="selector_control" class="btn-group pb-3">
        <button id="btn_student" type="button" class="btn btn-secondary border-dark" onclick="selectorClicked('student');">Student</button>
        <button id="btn_professor" type="button" class="btn btn-secondary border-dark" onclick="selectorClicked('professor');">Professor</button>
    </div>

    <div id="row_userid" class="form-group">
        <label id="lbl_login_id" for="txt_login_id">User</label>
        <input type="text" class="form-control w-25" id="txt_login_id" placeholder="Enter User ID">
    </div>
    <div id="row_password" class="form-group">
        <label id="lbl_pass_id" for="txt_pass">Password</label>
        <input type="password" class="form-control w-25" id="txt_pass" placeholder="Password">
    </div>

    <input id="btn_submit" type="button" class="btn btn-primary pr-3" value="Submit" onclick="submitClicked();" />
    <a class="nav-link" href="#">Forgot password???</a>

</asp:Content>
