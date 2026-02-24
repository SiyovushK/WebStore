namespace Domain.Constants;

public record ErrorDetails(string Message, int StatusCode);

public static class ErrorMessages
{
    public static readonly ErrorDetails Invalid_Credentials = new("Invalid_Credentials", 401);
    public static readonly ErrorDetails User_Not_Found = new("User_Not_Found", 404);
    public static readonly ErrorDetails Email_Already_Exists = new("Email_Already_Exists", 400);
    public static readonly ErrorDetails Product_Not_Found = new("Product_Not_Found", 404);
    public static readonly ErrorDetails Item_Not_Found = new("Item_Not_Found", 404);
    public static readonly ErrorDetails Field_Is_Required = new("Field_Is_Required", 400);
    public static readonly ErrorDetails Invalid_Refresh_Token = new("Invalid_Refresh_Token", 401);
    public static readonly ErrorDetails Invalid_Name_Input = new("Invalid_Name_Input", 400);
    public static readonly ErrorDetails Invalid_Role_Input = new("Invalid_Role_Input", 400);
    public static readonly ErrorDetails Invalid_Email_Input = new("Invalid_Email_Input", 400);
    public static readonly ErrorDetails Invalid_Password_Input = new("Invalid_Password_Input", 400);
    public static readonly ErrorDetails Unauthorized = new("Unauthorized", 401);
    public static readonly ErrorDetails Internal_Server_Error = new("Internal_Server_Error", 500);
    public static readonly ErrorDetails Insufficient_Stock = new("Insufficient_Stock", 400);
    public static readonly ErrorDetails Product_Unavailable = new("Product_Unavailable", 400);
    public static readonly ErrorDetails Item_Already_In_Cart = new("Item_Already_In_Cart", 400);
}