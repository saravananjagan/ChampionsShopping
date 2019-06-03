$(document).ready(function () {
    var quantitiy = 1;
    $('.quantity-right-plus').click(function (e) {

        // Stop acting like a button
        e.preventDefault();
        // Get the field name
        var quantity = parseInt($('#quantity').val());

        // If is not undefined

        $('#quantity').val(quantity + 1);


        // Increment

    });

    $('.quantity-left-minus').click(function (e) {
        // Get the field name
        var quantity = parseInt($('#quantity').val());

        // If is not undefined

        // Increment
        if (quantity > 1) {
            $('#quantity').val(quantity - 1);
        }
    });
});

function IncreaseCartQty(ProductPricingId) {
    // Get the field name
    var quantity = parseInt($("#quantity_" + ProductPricingId).val());

    // If is not undefined
    $("#quantity_" + ProductPricingId).val(quantity + 1);
        // Increment
    UpdateCartItem(ProductPricingId);
}

function DecreaseCartQty(ProductPricingId) {
    // Get the field name
    var quantity = parseInt($("#quantity_" + ProductPricingId).val());

    // If is not undefined

    // Increment
    if (quantity > 1) {
        $("#quantity_" + ProductPricingId).val(quantity - 1);
        UpdateCartItem(ProductPricingId);
    } else {
        DeleteCartItem(ProductPricingId);
    }
    
}

function AddCartItem(ProductPricingId, ProductName, ProductId, BuyPrice, SellPrice, profit) {
    $("#CartItemQuantity").val(1);
    $("#Cart_ProductName").html(ProductName + " - " + ProductId);
    $("#Cart_BuyPrice").html("Buy Price: Rs." + BuyPrice);
    $("#Cart_SellPrice").html("Sell Price: Rs." + SellPrice);
    $("#Cart_Profit").html("Profit: Rs." + profit);
    $("#Cart_ProductPricingId").val(ProductPricingId);
    $("#AddToCartModal").modal();
}

function UpdateCartItem(ProductPricingId) {
    var CartItemQuantity = $("#quantity_"+ProductPricingId).val();
    //$("#AddToCartModal").modal('toggle');
    $.ajax({
        url: '../Sales/UpdateCartItem',
        type: 'POST',
        data: { ProductPricingId: ProductPricingId, CartItemQuantity: CartItemQuantity },
        success: function (data) {
            $("#ChampionsPricingGrid").html(data);
            GetCartCount();
        },
        error: function (e) {
            alert("Something wrong. Please check the internet connection!!!");
        }
    });
}

function DeleteCartItem(ProductPricingId) {
    $.ajax({
        url: '../Sales/DeleteCartItem',
        type: 'POST',
        data: { ProductPricingId: ProductPricingId },
        success: function (data) {
            $("#ChampionsPricingGrid").html(data);
            GetCartCount();
        },
        error: function (e) {
            alert("Something wrong. Please check the internet connection!!!");
        }
    });
}

function GetCartCount() {
    $.ajax({
        url: '../Sales/GetCartCount',
        type: 'GET',
        success: function (data) {
            var cartElement = $("#Cart");
            var count = data;
            var checkoutElement = "<i class=\"fa fa-shopping-cart\"></i>&nbsp;Checkout(" + count + ")";
            cartElement.html(checkoutElement);
        },
        error: function (e) {
            alert("Something wrong. Please check the internet connection!!!");
        }
    });
}