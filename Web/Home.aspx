<%@ Page Language="C#" MasterPageFile="~/global.Master" AutoEventWireup="true" CodeBehind="Home.aspx.cs" Inherits="gradproj_webapp.home" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <style type="text/css">
        .otptextarea {
            font-size : 120px;
        }

    </style>

    <script src="js/Home.js" type="text/javascript"></script>
    <script type="text/javascript">
        var Profile_Object;
        var Course_Object;

        $(document).ready(function () {
            var stack = "(HomeASPX)documentready";
            try {
                initiateHome(stack);
            }
            catch (ex) {
                appendLog(stack, ex.message);
            }
        });
    </script>
    
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    
    <div id="lable_userid" class="pb-2">
        <lable id="lbl_user" class="h5 text-capitalize"></lable>

    </div>
    <div id="class_control" class="input-group pr-3">
        <div class="input-group-prepend">
            <label id="lbl_class" class="input-group-text" for="select_month">Class</label>
        </div>
        <select class="custom-select" id="select_class">
            <option value="0">Select one.....</option>
        </select>
    </div>    
    
    <div id="otpcontainer" class="otp container-fluid center-block">
        <lable id="lbl_otp" class="otptextarea"></lable>
        <br/>
        <button type="button"  class="btn btn-primary" id="btn_generateotp" onclick="generateOTP();">Generate OTP</button>
    </div>
</asp:Content>