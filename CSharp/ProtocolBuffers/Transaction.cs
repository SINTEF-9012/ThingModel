//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from: Transaction.proto
// Note: requires additional types generated from: StringDeclaration.proto
// Note: requires additional types generated from: Thing.proto
// Note: requires additional types generated from: ThingType.proto
namespace ThingModel.Proto
{
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"Transaction")]
  public partial class Transaction : global::ProtoBuf.IExtensible
  {
    public Transaction() {}
    
    private readonly global::System.Collections.Generic.List<ThingModel.Proto.StringDeclaration> _string_declarations = new global::System.Collections.Generic.List<ThingModel.Proto.StringDeclaration>();
    [global::ProtoBuf.ProtoMember(1, Name=@"string_declarations", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public global::System.Collections.Generic.List<ThingModel.Proto.StringDeclaration> string_declarations
    {
      get { return _string_declarations; }
    }
  
    private readonly global::System.Collections.Generic.List<ThingModel.Proto.Thing> _things_publish_list = new global::System.Collections.Generic.List<ThingModel.Proto.Thing>();
    [global::ProtoBuf.ProtoMember(2, Name=@"things_publish_list", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public global::System.Collections.Generic.List<ThingModel.Proto.Thing> things_publish_list
    {
      get { return _things_publish_list; }
    }
  
    private readonly global::System.Collections.Generic.List<int> _things_remove_list = new global::System.Collections.Generic.List<int>();
    [global::ProtoBuf.ProtoMember(3, Name=@"things_remove_list", DataFormat = global::ProtoBuf.DataFormat.TwosComplement, Options = global::ProtoBuf.MemberSerializationOptions.Packed)]
    public global::System.Collections.Generic.List<int> things_remove_list
    {
      get { return _things_remove_list; }
    }
  
    private readonly global::System.Collections.Generic.List<ThingModel.Proto.ThingType> _thingtypes_declaration_list = new global::System.Collections.Generic.List<ThingModel.Proto.ThingType>();
    [global::ProtoBuf.ProtoMember(4, Name=@"thingtypes_declaration_list", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public global::System.Collections.Generic.List<ThingModel.Proto.ThingType> thingtypes_declaration_list
    {
      get { return _thingtypes_declaration_list; }
    }
  
    private int _string_sender_id;
    [global::ProtoBuf.ProtoMember(5, IsRequired = true, Name=@"string_sender_id", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public int string_sender_id
    {
      get { return _string_sender_id; }
      set { _string_sender_id = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
}