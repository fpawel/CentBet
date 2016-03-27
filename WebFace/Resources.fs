module Resources

open WebSharper.Core.Resources

type UtilsJs() =
    inherit BaseResource("utils.js")
        
type CouponCss() =
    inherit BaseResource("coupon.css")

type W3Css() =
    inherit BaseResource("http://www.w3schools.com/lib/w3.css")

type MaterialIcons() = 
    inherit BaseResource("material-icons.css")
    