// Generated by the protocol buffer compiler.  DO NOT EDIT!
// source: StringDeclaration.proto

package org.thingmodel.proto;

public final class ProtoStringDeclaration {
  private ProtoStringDeclaration() {}
  public static void registerAllExtensions(
      com.google.protobuf.ExtensionRegistry registry) {
  }
  public interface StringDeclarationOrBuilder
      extends com.google.protobuf.MessageOrBuilder {

    // required string value = 1;
    /**
     * <code>required string value = 1;</code>
     */
    boolean hasValue();
    /**
     * <code>required string value = 1;</code>
     */
    String getValue();
    /**
     * <code>required string value = 1;</code>
     */
    com.google.protobuf.ByteString
        getValueBytes();

    // required int32 key = 2;
    /**
     * <code>required int32 key = 2;</code>
     */
    boolean hasKey();
    /**
     * <code>required int32 key = 2;</code>
     */
    int getKey();
  }
  /**
   * Protobuf type {@code ThingModel.Proto.StringDeclaration}
   *
   * <pre>
   **
   *	In order to reduce networks exchanges,
   *	a string must be sent only one time
   *	in the connection lifetime.
   *
   *	The value is the string and the key is a unique
   *	number representing the string.
   *
   *	The key must not be a hash like a truncated
   *	md5 or sha1. Collisions risks are important
   *	with a 32bits key.
   *
   *	Instead, it's better to use a counter. 0 for
   *	the first string decleration, 1 for the second…
   *	And the Protocol Buffers encoding is more efficient
   *	with small numbers.
   *
   *	Each connection should use two dictionaries
   *	between these declarations. One dictionary
   *	for the emission and another one for the reception.
   *
   *	A string declaration can replace a previous one.
   *	It is not possible to remove a string declaration.
   *	Duplicates, the same string with differents keys, are
   *	allowed but developers must try to avoid that.
   * </pre>
   */
  public static final class StringDeclaration extends
      com.google.protobuf.GeneratedMessage
      implements StringDeclarationOrBuilder {
    // Use StringDeclaration.newBuilder() to construct.
    private StringDeclaration(com.google.protobuf.GeneratedMessage.Builder<?> builder) {
      super(builder);
      this.unknownFields = builder.getUnknownFields();
    }
    private StringDeclaration(boolean noInit) { this.unknownFields = com.google.protobuf.UnknownFieldSet.getDefaultInstance(); }

    private static final StringDeclaration defaultInstance;
    public static StringDeclaration getDefaultInstance() {
      return defaultInstance;
    }

    public StringDeclaration getDefaultInstanceForType() {
      return defaultInstance;
    }

    private final com.google.protobuf.UnknownFieldSet unknownFields;
    @Override
    public final com.google.protobuf.UnknownFieldSet
        getUnknownFields() {
      return this.unknownFields;
    }
    private StringDeclaration(
        com.google.protobuf.CodedInputStream input,
        com.google.protobuf.ExtensionRegistryLite extensionRegistry)
        throws com.google.protobuf.InvalidProtocolBufferException {
      initFields();
      int mutable_bitField0_ = 0;
      com.google.protobuf.UnknownFieldSet.Builder unknownFields =
          com.google.protobuf.UnknownFieldSet.newBuilder();
      try {
        boolean done = false;
        while (!done) {
          int tag = input.readTag();
          switch (tag) {
            case 0:
              done = true;
              break;
            default: {
              if (!parseUnknownField(input, unknownFields,
                                     extensionRegistry, tag)) {
                done = true;
              }
              break;
            }
            case 10: {
              bitField0_ |= 0x00000001;
              value_ = input.readBytes();
              break;
            }
            case 16: {
              bitField0_ |= 0x00000002;
              key_ = input.readInt32();
              break;
            }
          }
        }
      } catch (com.google.protobuf.InvalidProtocolBufferException e) {
        throw e.setUnfinishedMessage(this);
      } catch (java.io.IOException e) {
        throw new com.google.protobuf.InvalidProtocolBufferException(
            e.getMessage()).setUnfinishedMessage(this);
      } finally {
        this.unknownFields = unknownFields.build();
        makeExtensionsImmutable();
      }
    }
    public static final com.google.protobuf.Descriptors.Descriptor
        getDescriptor() {
      return ProtoStringDeclaration.internal_static_ThingModel_Proto_StringDeclaration_descriptor;
    }

    protected com.google.protobuf.GeneratedMessage.FieldAccessorTable
        internalGetFieldAccessorTable() {
      return ProtoStringDeclaration.internal_static_ThingModel_Proto_StringDeclaration_fieldAccessorTable
          .ensureFieldAccessorsInitialized(
              StringDeclaration.class, Builder.class);
    }

    public static com.google.protobuf.Parser<StringDeclaration> PARSER =
        new com.google.protobuf.AbstractParser<StringDeclaration>() {
      public StringDeclaration parsePartialFrom(
          com.google.protobuf.CodedInputStream input,
          com.google.protobuf.ExtensionRegistryLite extensionRegistry)
          throws com.google.protobuf.InvalidProtocolBufferException {
        return new StringDeclaration(input, extensionRegistry);
      }
    };

    @Override
    public com.google.protobuf.Parser<StringDeclaration> getParserForType() {
      return PARSER;
    }

    private int bitField0_;
    // required string value = 1;
    public static final int VALUE_FIELD_NUMBER = 1;
    private Object value_;
    /**
     * <code>required string value = 1;</code>
     */
    public boolean hasValue() {
      return ((bitField0_ & 0x00000001) == 0x00000001);
    }
    /**
     * <code>required string value = 1;</code>
     */
    public String getValue() {
      Object ref = value_;
      if (ref instanceof String) {
        return (String) ref;
      } else {
        com.google.protobuf.ByteString bs = 
            (com.google.protobuf.ByteString) ref;
        String s = bs.toStringUtf8();
        if (bs.isValidUtf8()) {
          value_ = s;
        }
        return s;
      }
    }
    /**
     * <code>required string value = 1;</code>
     */
    public com.google.protobuf.ByteString
        getValueBytes() {
      Object ref = value_;
      if (ref instanceof String) {
        com.google.protobuf.ByteString b = 
            com.google.protobuf.ByteString.copyFromUtf8(
                (String) ref);
        value_ = b;
        return b;
      } else {
        return (com.google.protobuf.ByteString) ref;
      }
    }

    // required int32 key = 2;
    public static final int KEY_FIELD_NUMBER = 2;
    private int key_;
    /**
     * <code>required int32 key = 2;</code>
     */
    public boolean hasKey() {
      return ((bitField0_ & 0x00000002) == 0x00000002);
    }
    /**
     * <code>required int32 key = 2;</code>
     */
    public int getKey() {
      return key_;
    }

    private void initFields() {
      value_ = "";
      key_ = 0;
    }
    private byte memoizedIsInitialized = -1;
    public final boolean isInitialized() {
      byte isInitialized = memoizedIsInitialized;
      if (isInitialized != -1) return isInitialized == 1;

      if (!hasValue()) {
        memoizedIsInitialized = 0;
        return false;
      }
      if (!hasKey()) {
        memoizedIsInitialized = 0;
        return false;
      }
      memoizedIsInitialized = 1;
      return true;
    }

    public void writeTo(com.google.protobuf.CodedOutputStream output)
                        throws java.io.IOException {
      getSerializedSize();
      if (((bitField0_ & 0x00000001) == 0x00000001)) {
        output.writeBytes(1, getValueBytes());
      }
      if (((bitField0_ & 0x00000002) == 0x00000002)) {
        output.writeInt32(2, key_);
      }
      getUnknownFields().writeTo(output);
    }

    private int memoizedSerializedSize = -1;
    public int getSerializedSize() {
      int size = memoizedSerializedSize;
      if (size != -1) return size;

      size = 0;
      if (((bitField0_ & 0x00000001) == 0x00000001)) {
        size += com.google.protobuf.CodedOutputStream
          .computeBytesSize(1, getValueBytes());
      }
      if (((bitField0_ & 0x00000002) == 0x00000002)) {
        size += com.google.protobuf.CodedOutputStream
          .computeInt32Size(2, key_);
      }
      size += getUnknownFields().getSerializedSize();
      memoizedSerializedSize = size;
      return size;
    }

    private static final long serialVersionUID = 0L;
    @Override
    protected Object writeReplace()
        throws java.io.ObjectStreamException {
      return super.writeReplace();
    }

    public static StringDeclaration parseFrom(
        com.google.protobuf.ByteString data)
        throws com.google.protobuf.InvalidProtocolBufferException {
      return PARSER.parseFrom(data);
    }
    public static StringDeclaration parseFrom(
        com.google.protobuf.ByteString data,
        com.google.protobuf.ExtensionRegistryLite extensionRegistry)
        throws com.google.protobuf.InvalidProtocolBufferException {
      return PARSER.parseFrom(data, extensionRegistry);
    }
    public static StringDeclaration parseFrom(byte[] data)
        throws com.google.protobuf.InvalidProtocolBufferException {
      return PARSER.parseFrom(data);
    }
    public static StringDeclaration parseFrom(
        byte[] data,
        com.google.protobuf.ExtensionRegistryLite extensionRegistry)
        throws com.google.protobuf.InvalidProtocolBufferException {
      return PARSER.parseFrom(data, extensionRegistry);
    }
    public static StringDeclaration parseFrom(java.io.InputStream input)
        throws java.io.IOException {
      return PARSER.parseFrom(input);
    }
    public static StringDeclaration parseFrom(
        java.io.InputStream input,
        com.google.protobuf.ExtensionRegistryLite extensionRegistry)
        throws java.io.IOException {
      return PARSER.parseFrom(input, extensionRegistry);
    }
    public static StringDeclaration parseDelimitedFrom(java.io.InputStream input)
        throws java.io.IOException {
      return PARSER.parseDelimitedFrom(input);
    }
    public static StringDeclaration parseDelimitedFrom(
        java.io.InputStream input,
        com.google.protobuf.ExtensionRegistryLite extensionRegistry)
        throws java.io.IOException {
      return PARSER.parseDelimitedFrom(input, extensionRegistry);
    }
    public static StringDeclaration parseFrom(
        com.google.protobuf.CodedInputStream input)
        throws java.io.IOException {
      return PARSER.parseFrom(input);
    }
    public static StringDeclaration parseFrom(
        com.google.protobuf.CodedInputStream input,
        com.google.protobuf.ExtensionRegistryLite extensionRegistry)
        throws java.io.IOException {
      return PARSER.parseFrom(input, extensionRegistry);
    }

    public static Builder newBuilder() { return Builder.create(); }
    public Builder newBuilderForType() { return newBuilder(); }
    public static Builder newBuilder(StringDeclaration prototype) {
      return newBuilder().mergeFrom(prototype);
    }
    public Builder toBuilder() { return newBuilder(this); }

    @Override
    protected Builder newBuilderForType(
        com.google.protobuf.GeneratedMessage.BuilderParent parent) {
      Builder builder = new Builder(parent);
      return builder;
    }
    /**
     * Protobuf type {@code ThingModel.Proto.StringDeclaration}
     *
     * <pre>
     **
     *	In order to reduce networks exchanges,
     *	a string must be sent only one time
     *	in the connection lifetime.
     *
     *	The value is the string and the key is a unique
     *	number representing the string.
     *
     *	The key must not be a hash like a truncated
     *	md5 or sha1. Collisions risks are important
     *	with a 32bits key.
     *
     *	Instead, it's better to use a counter. 0 for
     *	the first string decleration, 1 for the second…
     *	And the Protocol Buffers encoding is more efficient
     *	with small numbers.
     *
     *	Each connection should use two dictionaries
     *	between these declarations. One dictionary
     *	for the emission and another one for the reception.
     *
     *	A string declaration can replace a previous one.
     *	It is not possible to remove a string declaration.
     *	Duplicates, the same string with differents keys, are
     *	allowed but developers must try to avoid that.
     * </pre>
     */
    public static final class Builder extends
        com.google.protobuf.GeneratedMessage.Builder<Builder>
       implements StringDeclarationOrBuilder {
      public static final com.google.protobuf.Descriptors.Descriptor
          getDescriptor() {
        return ProtoStringDeclaration.internal_static_ThingModel_Proto_StringDeclaration_descriptor;
      }

      protected com.google.protobuf.GeneratedMessage.FieldAccessorTable
          internalGetFieldAccessorTable() {
        return ProtoStringDeclaration.internal_static_ThingModel_Proto_StringDeclaration_fieldAccessorTable
            .ensureFieldAccessorsInitialized(
                StringDeclaration.class, Builder.class);
      }

      // Construct using org.thingmodel.proto.ProtoStringDeclaration.StringDeclaration.newBuilder()
      private Builder() {
        maybeForceBuilderInitialization();
      }

      private Builder(
          com.google.protobuf.GeneratedMessage.BuilderParent parent) {
        super(parent);
        maybeForceBuilderInitialization();
      }
      private void maybeForceBuilderInitialization() {
        if (com.google.protobuf.GeneratedMessage.alwaysUseFieldBuilders) {
        }
      }
      private static Builder create() {
        return new Builder();
      }

      public Builder clear() {
        super.clear();
        value_ = "";
        bitField0_ = (bitField0_ & ~0x00000001);
        key_ = 0;
        bitField0_ = (bitField0_ & ~0x00000002);
        return this;
      }

      public Builder clone() {
        return create().mergeFrom(buildPartial());
      }

      public com.google.protobuf.Descriptors.Descriptor
          getDescriptorForType() {
        return ProtoStringDeclaration.internal_static_ThingModel_Proto_StringDeclaration_descriptor;
      }

      public StringDeclaration getDefaultInstanceForType() {
        return StringDeclaration.getDefaultInstance();
      }

      public StringDeclaration build() {
        StringDeclaration result = buildPartial();
        if (!result.isInitialized()) {
          throw newUninitializedMessageException(result);
        }
        return result;
      }

      public StringDeclaration buildPartial() {
        StringDeclaration result = new StringDeclaration(this);
        int from_bitField0_ = bitField0_;
        int to_bitField0_ = 0;
        if (((from_bitField0_ & 0x00000001) == 0x00000001)) {
          to_bitField0_ |= 0x00000001;
        }
        result.value_ = value_;
        if (((from_bitField0_ & 0x00000002) == 0x00000002)) {
          to_bitField0_ |= 0x00000002;
        }
        result.key_ = key_;
        result.bitField0_ = to_bitField0_;
        onBuilt();
        return result;
      }

      public Builder mergeFrom(com.google.protobuf.Message other) {
        if (other instanceof StringDeclaration) {
          return mergeFrom((StringDeclaration)other);
        } else {
          super.mergeFrom(other);
          return this;
        }
      }

      public Builder mergeFrom(StringDeclaration other) {
        if (other == StringDeclaration.getDefaultInstance()) return this;
        if (other.hasValue()) {
          bitField0_ |= 0x00000001;
          value_ = other.value_;
          onChanged();
        }
        if (other.hasKey()) {
          setKey(other.getKey());
        }
        this.mergeUnknownFields(other.getUnknownFields());
        return this;
      }

      public final boolean isInitialized() {
        if (!hasValue()) {
          
          return false;
        }
        if (!hasKey()) {
          
          return false;
        }
        return true;
      }

      public Builder mergeFrom(
          com.google.protobuf.CodedInputStream input,
          com.google.protobuf.ExtensionRegistryLite extensionRegistry)
          throws java.io.IOException {
        StringDeclaration parsedMessage = null;
        try {
          parsedMessage = PARSER.parsePartialFrom(input, extensionRegistry);
        } catch (com.google.protobuf.InvalidProtocolBufferException e) {
          parsedMessage = (StringDeclaration) e.getUnfinishedMessage();
          throw e;
        } finally {
          if (parsedMessage != null) {
            mergeFrom(parsedMessage);
          }
        }
        return this;
      }
      private int bitField0_;

      // required string value = 1;
      private Object value_ = "";
      /**
       * <code>required string value = 1;</code>
       */
      public boolean hasValue() {
        return ((bitField0_ & 0x00000001) == 0x00000001);
      }
      /**
       * <code>required string value = 1;</code>
       */
      public String getValue() {
        Object ref = value_;
        if (!(ref instanceof String)) {
          String s = ((com.google.protobuf.ByteString) ref)
              .toStringUtf8();
          value_ = s;
          return s;
        } else {
          return (String) ref;
        }
      }
      /**
       * <code>required string value = 1;</code>
       */
      public com.google.protobuf.ByteString
          getValueBytes() {
        Object ref = value_;
        if (ref instanceof String) {
          com.google.protobuf.ByteString b = 
              com.google.protobuf.ByteString.copyFromUtf8(
                  (String) ref);
          value_ = b;
          return b;
        } else {
          return (com.google.protobuf.ByteString) ref;
        }
      }
      /**
       * <code>required string value = 1;</code>
       */
      public Builder setValue(
          String value) {
        if (value == null) {
    throw new NullPointerException();
  }
  bitField0_ |= 0x00000001;
        value_ = value;
        onChanged();
        return this;
      }
      /**
       * <code>required string value = 1;</code>
       */
      public Builder clearValue() {
        bitField0_ = (bitField0_ & ~0x00000001);
        value_ = getDefaultInstance().getValue();
        onChanged();
        return this;
      }
      /**
       * <code>required string value = 1;</code>
       */
      public Builder setValueBytes(
          com.google.protobuf.ByteString value) {
        if (value == null) {
    throw new NullPointerException();
  }
  bitField0_ |= 0x00000001;
        value_ = value;
        onChanged();
        return this;
      }

      // required int32 key = 2;
      private int key_ ;
      /**
       * <code>required int32 key = 2;</code>
       */
      public boolean hasKey() {
        return ((bitField0_ & 0x00000002) == 0x00000002);
      }
      /**
       * <code>required int32 key = 2;</code>
       */
      public int getKey() {
        return key_;
      }
      /**
       * <code>required int32 key = 2;</code>
       */
      public Builder setKey(int value) {
        bitField0_ |= 0x00000002;
        key_ = value;
        onChanged();
        return this;
      }
      /**
       * <code>required int32 key = 2;</code>
       */
      public Builder clearKey() {
        bitField0_ = (bitField0_ & ~0x00000002);
        key_ = 0;
        onChanged();
        return this;
      }

      // @@protoc_insertion_point(builder_scope:ThingModel.Proto.StringDeclaration)
    }

    static {
      defaultInstance = new StringDeclaration(true);
      defaultInstance.initFields();
    }

    // @@protoc_insertion_point(class_scope:ThingModel.Proto.StringDeclaration)
  }

  private static com.google.protobuf.Descriptors.Descriptor
    internal_static_ThingModel_Proto_StringDeclaration_descriptor;
  private static
    com.google.protobuf.GeneratedMessage.FieldAccessorTable
      internal_static_ThingModel_Proto_StringDeclaration_fieldAccessorTable;

  public static com.google.protobuf.Descriptors.FileDescriptor
      getDescriptor() {
    return descriptor;
  }
  private static com.google.protobuf.Descriptors.FileDescriptor
      descriptor;
  static {
    String[] descriptorData = {
      "\n\027StringDeclaration.proto\022\020ThingModel.Pr" +
      "oto\"/\n\021StringDeclaration\022\r\n\005value\030\001 \002(\t\022" +
      "\013\n\003key\030\002 \002(\005B.\n\024org.thingmodel.protoB\026Pr" +
      "otoStringDeclaration"
    };
    com.google.protobuf.Descriptors.FileDescriptor.InternalDescriptorAssigner assigner =
      new com.google.protobuf.Descriptors.FileDescriptor.InternalDescriptorAssigner() {
        public com.google.protobuf.ExtensionRegistry assignDescriptors(
            com.google.protobuf.Descriptors.FileDescriptor root) {
          descriptor = root;
          internal_static_ThingModel_Proto_StringDeclaration_descriptor =
            getDescriptor().getMessageTypes().get(0);
          internal_static_ThingModel_Proto_StringDeclaration_fieldAccessorTable = new
            com.google.protobuf.GeneratedMessage.FieldAccessorTable(
              internal_static_ThingModel_Proto_StringDeclaration_descriptor,
              new String[] { "Value", "Key", });
          return null;
        }
      };
    com.google.protobuf.Descriptors.FileDescriptor
      .internalBuildGeneratedFileFrom(descriptorData,
        new com.google.protobuf.Descriptors.FileDescriptor[] {
        }, assigner);
  }

  // @@protoc_insertion_point(outer_class_scope)
}
