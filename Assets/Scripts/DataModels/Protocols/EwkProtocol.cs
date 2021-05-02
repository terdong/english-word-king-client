using System;
using System.Linq;
using System.Text;
using ProtoBuf;
using System.IO;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.IO.Compression;
using System.Collections.Generic;

// Client와 namespace 맞추기위해 고정으로함.
namespace TeamGehem.DataModels.Protocols
{
    [ProtoContract]
    [ProtoInclude( 100, typeof( EwkProtocol ) )]
    [ProtoInclude( 101, typeof( EwkProtocol<string> ) )]
    [ProtoInclude( 102, typeof( EwkProtocol<int> ) )]
    [ProtoInclude( 103, typeof( EwkProtocol<float> ) )]
    [ProtoInclude( 104, typeof( EwkProtocol<IDictionary<string,string>> ) )]
    [ProtoInclude( 105, typeof( EwkProtocol<IDictionary<string,int>> ) )]
    [ProtoInclude( 106, typeof( EwkProtocol<IDictionary<string,float>> ) )]
    [ProtoInclude( 107, typeof( EwkProtocol<IList<string>> ) )]
    [ProtoInclude( 108, typeof( EwkProtocol<IList<int>> ) )]
    [ProtoInclude( 109, typeof( EwkProtocol<IList<float>> ) )]
    [ProtoInclude( 110, typeof( EwkProtocol<QuizInfo> ) )]
    [ProtoInclude( 111, typeof( EwkProtocol<AvatarInfo> ) )]
    [ProtoInclude( 112, typeof( EwkProtocol<FightInfo> ) )]
    [ProtoInclude( 113, typeof( EwkProtocol<UserInfo> ) )]
    [ProtoInclude(114, typeof(EwkProtocol<SceneListEnum>))]
    public abstract class IEwkProtocol
    {
        [ProtoMember(1)]
        public abstract ProtocolEnum Protocol_Enum { set; get; }

        public abstract byte[] GetBytes { get; }

        public virtual T GetData<T>()
        {
            return default( T );
        }
    }

    [ProtoContract]
    public class EwkProtocol : IEwkProtocol
    {
        [ProtoMember(1)]
        public override ProtocolEnum Protocol_Enum { set; get; }

        public override byte[] GetBytes
        {
            get { return EwkProtoSerilazer.SerializeForProtobuf_<IEwkProtocol>( this ); }
        }

        public EwkProtocol() { }

        public override string ToString()
        {
            return Protocol_Enum.ToString();
        }
    }

    [ProtoContract]
    public class EwkProtocol<T> : IEwkProtocol
    {
        [ProtoMember( 1 )]
        public override ProtocolEnum Protocol_Enum { set; get; }

        [ProtoMember(2)]
        public T Data { set; get; }

        public override byte[] GetBytes
        {
            get { return EwkProtoSerilazer.SerializeForProtobuf_<EwkProtocol<T>>( this ); }
        }

        public EwkProtocol() { }

        public override string ToString()
        {
            return Protocol_Enum.ToString();
        }

        public override G GetData<G>()
        {
            return ( this as EwkProtocol<G> ).Data;
        }
    }

    public class EwkProtoSerilazer
    {
        /// <summary>
        /// EwkProtocol Class를 상속받은 Class(T형) 만 Serialization 이 가능함.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="target"></param>
        /// <returns></returns>
        public static byte[] SerializeForProtobuf_<T>( T target )
        {
            byte[] byte_array = null;

            using ( MemoryStream serialize = new MemoryStream() )
            {
                ProtoBuf.Serializer.Serialize<T>( serialize, target );

                byte_array = serialize.ToArray();
            }
            return byte_array;
        }

        private static T DeserializeForProtobuf_<T>( byte[] binary_data )
        {
            T t_object = default(T);
            using ( Stream stream = new MemoryStream( binary_data ) )
            {
                t_object = ProtoBuf.Serializer.Deserialize<T>( stream );
            }
            return t_object;
        }

        public static IEwkProtocol DeserializeForProtobuf( byte[] binary_data )
        {
            return DeserializeForProtobuf_<IEwkProtocol>( binary_data );
        }

        public static T DeserializeForData<T>( byte[] binary_data )
        {
            return DeserializeForProtobuf_<EwkProtocol<T>>( binary_data ).Data;
        }
    }

    public class EwkProtoFactory
    {
        public static IEwkProtocol CreateEwkProtocol( ProtocolEnum protocol_enum )
        {
            return new EwkProtocol()
            {
                Protocol_Enum = protocol_enum,
            };
        }

        public static IEwkProtocol CreateEwkProtocol<T>( ProtocolEnum protocol_enum, T data )
        {
            return new EwkProtocol<T>()
            {
                Protocol_Enum = protocol_enum,
                Data = data
            };
        }
    }
}
