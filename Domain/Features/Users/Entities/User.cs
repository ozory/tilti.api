using Domain.Abstractions;
using Domain.Enums;
using Domain.Features.Orders.Entities;
using Domain.Features.Subscriptions.Entities;
using Domain.Features.Users.Events;
using Domain.Shared.Abstractions;
using Domain.ValueObjects;

namespace Domain.Features.Users.Entities;

/// <summary>
/// Represents an user
/// </summary>
public class User : Entity
{
    #region PROPERTIES

    public Name Name { get; protected set; } = null!;
    public Boolean DriveEnable { get; protected set; } = false;
    public Transport? Transport { get; protected set; }
    public Email Email { get; protected set; } = null!;
    public Document Document { get; protected set; } = null!;
    public Password? Password { get; protected set; } = null!;
    public UserStatus Status { get; protected set; }
    public string? PaymentToken { get; protected set; }
    public string? PaymentUserIdentifier { get; protected set; }

    public string? Photo { get; protected set; }
    public string? VerificationCode { get; protected set; }
    public string? VerificationSalt { get; protected set; }

    public virtual ICollection<Order> UserOrders { get; protected set; } = null!;
    public virtual ICollection<Order> DriverOrders { get; protected set; } = null!;

    public virtual ICollection<Rate> SourceRates { get; protected set; } = null!;
    public virtual ICollection<Rate> TargetRates { get; protected set; } = null!;

    public virtual Subscription? Subscription { get; protected set; } = null!;
    public virtual ICollection<Rejection> Rejections { get; protected set; } = [];
    public virtual ICollection<Message> SourceMessages { get; protected set; } = [];
    public virtual ICollection<Message> TargetMessages { get; protected set; } = [];

    #endregion

    #region CONSTRUCTORS

    private User() { }

    /// <summary>
    /// Cria um novo usuário
    /// </summary>
    /// <param name="name"></param>
    /// <param name="email"></param>
    /// <param name="document"></param>
    /// <param name="password"></param> 
    /// <returns>Retorna um novo usuário</returns>
    private User(long? id, Name name, Email email, Document document, Password? password)
    {
        Id = id ?? 0;
        Name = name;
        Email = email;
        Document = document;
        Password = password;
    }

    private User(long? id)
    {
        Id = id ?? 0;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <param name="email"></param>
    /// <param name="document"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    public static User Create(
        long? id,
        string name,
        string email,
        string document,
        string? password,
        DateTime? createdDate)
    {
        var user = new User(
            id,
            new Name(name),
            new Email(email),
            new Document(document),
            new Password(password));

        user.CreatedAt = createdDate ?? DateTime.Now;

        return user;
    }

    public static User Create(long? id)
    {
        var user = new User(id);
        return user;
    }

    #endregion

    #region METHODS

    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    public void SetName(string name) => this.Name.SetValue(name);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="email"></param>
    public void SetEmail(string email) => this.Email.SetValue(email);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="document"></param>
    public void SetDocument(string document) => this.Document.SetValue(document);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="userStatus"></param>
    public void SetStatus(UserStatus userStatus) => this.Status = userStatus;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="updatedAt"></param>
    public void SetUpdatedAt(DateTime updatedAt) => this.UpdatedAt = updatedAt;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="photo"></param>
    public void SetPhoto(string? photo) => this.Photo = photo;

    public void SetPassword(string password) => this.Password!.SetValue(password);

    /// <summary>
    /// Indicates when a user can be a drive
    /// </summary>
    /// <param name="driveEnable"></param>
    public void SetDriveEnable(bool driveEnable) => this.DriveEnable = driveEnable;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <param name="description"></param>
    /// <param name="year"></param>
    /// <param name="model"></param>
    /// <param name="plate"></param>
    public void SetTransport(string name, string description, ushort year, string model, string plate)
    {
        this.Transport = Transport.CreateTransport(name,
            description,
            year,
            plate,
            model);

    }

    public void SetVerificationCode(string verificationCode)
        => this.VerificationCode = verificationCode;

    public void SetVerificationSalt(string verificationSalt)
    => this.VerificationSalt = verificationSalt;

    public void SetPaymentToken(string? paymentToken)
    => this.PaymentToken = paymentToken;

    public void SetPaymentUserIdentifier(string? paymentUserIdentifier)
    => this.PaymentUserIdentifier = paymentUserIdentifier;

    #endregion METHODS
}
