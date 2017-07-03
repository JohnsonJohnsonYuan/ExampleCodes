# Contents

## Key points

1. **通过Account.MyAccount属性来得到当前用户, 保存在Session中**

```cs
Account myAccount = Account.MyAccount;
```

1. **通过Order.MyOrder.cart属性来得到当前用户购物车, 保存在Session中**

```cs
Cart myCart = Order.MyOrder.cart;

// \Components\Order.cs
[XmlIgnore]
public Cart cart = new Cart();

[XmlIgnore]
public static Order MyOrder {
    get {
        Order myOrder = HttpContext.Current.Session[KEY_ORDER] as Order;
        if (myOrder == null)
            myOrder = MyOrder = new Order();
        return myOrder;
    }
    set { HttpContext.Current.Session[KEY_ORDER] = value; }
}
```

1. cart 部分关键定义

```cs
public class Cart : IEnumerable {
    private ArrayList items = new ArrayList();

    public int Count {
        get { return items.Count; }
    }

    public CartItem this[int index] {
        get { return (CartItem)items[index]; }
    }

    public void Add(string itemId) {
        foreach (CartItem item in items) {
            if (itemId == item.Item.Id) {
                item.Quantity++;
                return;
            }
        }

        items.Add(new CartItem(itemId));
    }

    public decimal GetTotal() {
        decimal total = 0;

        foreach (CartItem item in items)
            total += item.Subtotal;

        return total;
    }
}
```

## Sign in(SignIn.aspx.cs)

```xml
    <authentication mode="Forms">
      <forms name="PetShopAuth" loginUrl="SignIn.aspx" protection="None" timeout="60" />
    </authentication>
```

```cs
bool success = Account.SignIn(userId, txtPassword.Text);
if (success) {
    if (FormsAuthentication.GetRedirectUrl(userId, false).EndsWith(URL_DEFAULT)) {
        FormsAuthentication.SetAuthCookie(userId, false);
        Server.Transfer(URL_SIGN_IN);
    }
    else
        FormsAuthentication.RedirectFromLoginPage(userId, false);
}
else {
    valUserId.ErrorMessage = MSG_FAILURE;
    valUserId.IsValid = false;
}

// Account.cs SignIn 方法 调用
// MyAccount = new Account(userId, password, rdr.GetString(0), ... )
public static Account MyAccount {
    get { return (Account)HttpContext.Current.Session[KEY_ACCOUNT]; }
    set { HttpContext.Current.Session[KEY_ACCOUNT] = value; }
}

```

## Add item to cart(ShoppingCart.aspx?itemId=FI-SW-01)

```cs
override protected void OnLoad(EventArgs e)
{
    myCart = Order.MyOrder.cart;

    if (!Page.IsPostBack)
    {
        if (Request[KEY_ITEM_ID] != null)
            myCart.Add(Request[KEY_ITEM_ID]);

        Account myAccount = Account.MyAccount;

        if (myAccount != null && myAccount.showFavorites)
        {
            favorites.Visible = true;
            ViewState[KEY_CATEGORY] = myAccount.category;
        }

        Refresh();
    }
}
```

## Process order(\Components\OrderCOM.cs)

> Method: ```public void Insert(Order order)```
1. Insert data to table **Order**
1. Inertt data to table **OrderStatus**
1. Insert data to table **LineItem**

```cs
int i = 1;

foreach (CartItem item in order.cart) {
    itemParms[0].Value = order.orderId;
    itemParms[1].Value = i++;
    itemParms[2].Value = item.Item.Id;
    itemParms[3].Value = item.Quantity;
    itemParms[4].Value = item.Item.Price;

    Database.ExecuteNonQuery(Database.CONN_STRING2, CommandType.Text, SQL_INSERT_ITEM, itemParms);

    inventoryParms[0].Value = item.Quantity;
    inventoryParms[1].Value = item.Item.Id;

    Database.ExecuteNonQuery(Database.CONN_STRING1, CommandType.Text, SQL_UPDATE_INVENTORY, inventoryParms);
}
```