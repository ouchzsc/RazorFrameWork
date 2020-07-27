using System;
using System.Text;

namespace Aio
{
    public sealed class MarshalException : Exception
    {
    }

    public sealed class OctetsStream
    {
        public Octets Data { get; private set; }

        public int Position { get; set; }        

        public int Remaining
        {
            get { return Data.Count - Position; }
        }


        public bool Eos
        {
            get { return Position == Data.Count; }
        }


        public OctetsStream()
        {
            Data = new Octets();
        }

        public OctetsStream(int size)
        {
            Data = new Octets(size);
        }

        public  OctetsStream Reset()
        {
            Data.Reset();
            Position = 0;
            return this;
        }

        public OctetsStream ResetWithSize(int reserveSize)
        {
            Data.ResetWithSize(reserveSize);
            Position = 0;
            return this;
        }


        public OctetsStream WrapOctets(Octets o)
        {
            Data = o;
            Position = 0;
            return this;
        }

        public static OctetsStream Wrap(Octets o)
        {
            return new OctetsStream(o);
        }

        private OctetsStream(Octets o)
        {
            Data = o;
        }

        public int Begin()
        {
            return Position;
        }

        public OctetsStream Rollback(int tranpos)
        {
            Position = tranpos;
            return this;
        }

        public OctetsStream MarshalBool(bool b)
        {
            Data.Append((byte)(b ? 1 : 0));
            return this;
        }

        public OctetsStream MarshalByte(byte x)
        {
            Data.Append(x);
            return this;
        }

        public OctetsStream MarshalSbyte(sbyte x)
        {
            Data.Append((byte)x);
            return this;
        }

        public OctetsStream MarshalUshort(ushort x)
        {
            return
            MarshalByte((byte)(x >> 8)).
            MarshalByte((byte)(x));
        }

        public OctetsStream MarshalShort(short x)
        {
            return
            MarshalByte((byte)(x >> 8)).
            MarshalByte((byte)(x));
        }

        public OctetsStream MarshalUint(uint x)
        {
            return
            MarshalByte((byte)(x >> 24)).
            MarshalByte((byte)(x >> 16)).
            MarshalByte((byte)(x >> 8)).
            MarshalByte((byte)(x));
        }

        public OctetsStream MarshalInt(int x)
        {
            return
            MarshalByte((byte)(x >> 24)).
            MarshalByte((byte)(x >> 16)).
            MarshalByte((byte)(x >> 8)).
            MarshalByte((byte)(x));
        }

        public OctetsStream MarshalUlong(ulong x)
        {
            return
            MarshalByte((byte)(x >> 56)).
            MarshalByte((byte)(x >> 48)).
            MarshalByte((byte)(x >> 40)).
            MarshalByte((byte)(x >> 32)).
            MarshalByte((byte)(x >> 24)).
            MarshalByte((byte)(x >> 16)).
            MarshalByte((byte)(x >> 8)).
            MarshalByte((byte)(x));
        }

        public OctetsStream MarshalLong(long x)
        {
            return
            MarshalByte((byte)(x >> 56)).
            MarshalByte((byte)(x >> 48)).
            MarshalByte((byte)(x >> 40)).
            MarshalByte((byte)(x >> 32)).
            MarshalByte((byte)(x >> 24)).
            MarshalByte((byte)(x >> 16)).
            MarshalByte((byte)(x >> 8)).
            MarshalByte((byte)(x));
        }

        public OctetsStream MarshalFloat(float x)
        {
            int v = 0;
            unsafe
            {
                v = *(int*)&x;
            }
            if (BitConverter.IsLittleEndian)
            {
                return
                    MarshalByte((byte)(v >> 24)).
                    MarshalByte((byte)(v >> 16)).
                    MarshalByte((byte)(v >> 8)).
                    MarshalByte((byte)(v));
            }
            return
                    MarshalByte((byte)(v)).
                    MarshalByte((byte)(v >> 8)).
                    MarshalByte((byte)(v >> 16)).
                    MarshalByte((byte)(v >> 24));
        }

        public OctetsStream MarshalDouble(double x)
        {
            long v = 0;
            unsafe
            {
                v = *(long*)&x;
            }
            if (BitConverter.IsLittleEndian)
            {
               return
                       MarshalByte((byte)(v >> 56)).
                       MarshalByte((byte)(v >> 48)).
                       MarshalByte((byte)(v >> 40)).
                       MarshalByte((byte)(v >> 32)).
                       MarshalByte((byte)(v >> 24)).
                       MarshalByte((byte)(v >> 16)).
                       MarshalByte((byte)(v >> 8)).
                       MarshalByte((byte)v);
            }
            return
                    MarshalByte((byte)(v)).
                    MarshalByte((byte)(v >> 8)).
                    MarshalByte((byte)(v >> 16)).
                    MarshalByte((byte)(v >> 24)).
                    MarshalByte((byte)(v >> 32)).
                    MarshalByte((byte)(v >> 40)).
                    MarshalByte((byte)(v >> 48)).
                    MarshalByte((byte)(v >> 56));
        }

        public OctetsStream MarshalOctets(Octets o)
        {
            MarshalSize(o.Count);
            Data.Append(o);
            return this;
        }

        public OctetsStream MarshalBytes(byte[] bytes)
        {
            MarshalSize(bytes.Length);
            Data.Append(bytes);
            return this;
        }

        public OctetsStream MarshalString(string str)
        {
            byte[] bytes = Encoding.Unicode.GetBytes(str);
            return MarshalBytes(bytes);
        }

        public OctetsStream MarshalSize(int size)
        {
            var x = (uint)size;
            if (x < 0x80)
                return MarshalByte((byte)x);
            if (x < 0x4000)
                return MarshalShort((short)(x | 0x8000));
            if (x < 0x20000000)
                return MarshalUint(x | 0xc0000000);
            return MarshalByte(0xe0).MarshalUint(x);
        }


        public bool UnmarshalBool()
        {
            return UnmarshalByte() == 1;
        }

        public byte UnmarshalByte()
        {
            if (Position + 1 > Data.Count)
                throw new MarshalException();
            return Data.GetByte(Position++);
        }

        public sbyte UnmarshalSbyte()
        {
            if (Position + 1 > Data.Count)
                throw new MarshalException();
            return (sbyte)Data.GetByte(Position++);
        }

        public ushort UnmarshalUshort()
        {
            if (Position + 2 > Data.Count)
                throw new MarshalException();
            byte b0 = Data.GetByte(Position++);
            byte b1 = Data.GetByte(Position++);
            return (ushort)((b0 << 8) | (b1 & 0xff));
        }

        public short UnmarshalShort()
        {
            if (Position + 2 > Data.Count)
                throw new MarshalException();
            byte b0 = Data.GetByte(Position++);
            byte b1 = Data.GetByte(Position++);
            return (short)((b0 << 8) | (b1 & 0xff));
        }

        public uint UnmarshalUint()
        {
            if (Position + 4 > Data.Count)
                throw new MarshalException();
            byte b0 = Data.GetByte(Position++);
            byte b1 = Data.GetByte(Position++);
            byte b2 = Data.GetByte(Position++);
            byte b3 = Data.GetByte(Position++);
            return (uint)(
                ((b0 & 0xff) << 24) |
                ((b1 & 0xff) << 16) |
                ((b2 & 0xff) << 8) |
                ((b3 & 0xff) << 0));
        }

        public int UnmarshalInt()
        {
            if (Position + 4 > Data.Count)
                throw new MarshalException();
            byte b0 = Data.GetByte(Position++);
            byte b1 = Data.GetByte(Position++);
            byte b2 = Data.GetByte(Position++);
            byte b3 = Data.GetByte(Position++);
            return ((b0 & 0xff) << 24) |
                   ((b1 & 0xff) << 16) |
                   ((b2 & 0xff) << 8) |
                   ((b3 & 0xff) << 0);
        }

        public ulong UnmarshalUlong()
        {
            if (Position + 8 > Data.Count)
                throw new MarshalException();
            byte b0 = Data.GetByte(Position++);
            byte b1 = Data.GetByte(Position++);
            byte b2 = Data.GetByte(Position++);
            byte b3 = Data.GetByte(Position++);
            byte b4 = Data.GetByte(Position++);
            byte b5 = Data.GetByte(Position++);
            byte b6 = Data.GetByte(Position++);
            byte b7 = Data.GetByte(Position++);
            return ((((ulong)b0) & 0xff) << 56) |
                   ((((ulong)b1) & 0xff) << 48) |
                   ((((ulong)b2) & 0xff) << 40) |
                   ((((ulong)b3) & 0xff) << 32) |
                   ((((ulong)b4) & 0xff) << 24) |
                   ((((ulong)b5) & 0xff) << 16) |
                   ((((ulong)b6) & 0xff) << 8) |
                   ((((ulong)b7) & 0xff) << 0);
        }

        public long UnmarshalLong()
        {
            return (long)UnmarshalUlong();
        }

        public float UnmarshalFloat()
        {
            if (Position + 4 > Data.Count)
                throw new MarshalException();

            int v = UnmarshalInt();
            float f = 0.0f;
            unsafe
            {
                f = *(float*) &v;
            }
            return f;
        }

        public double UnmarshalDouble()
        {
            if (Position + 8 > Data.Count)
                throw new MarshalException();

            long v = UnmarshalLong();
            double d = 0.0d;
            unsafe
            {
                d = *(double*)&v;
            }
            return d;
        }

        public Octets UnmarshalOctets()
        {
            int size = UnmarshalSize();
            if (Position + size > Data.Count)
                throw new MarshalException();
            var o = new Octets(Data, Position, size);
            Position += size;
            return o;
        }

        public byte[] UnmarshalBytes()
        {
            int size = UnmarshalSize();
            return UnmarshalFixedSizeBytes(size);
        }

        internal byte[] UnmarshalFixedSizeBytes(int size)
        {
            if (Position + size > Data.Count)
                throw new MarshalException();
            var copy = new byte[size];
            Buffer.BlockCopy(Data.ByteArray, Position, copy, 0, size);
            Position += size;
            return copy;
        }
        
        public string UnmarshalString()
        {
            byte[] bytes = UnmarshalBytes();
            return Encoding.Unicode.GetString(bytes);
        }


        public int UnmarshalSize()
        {
            if (Position == Data.Count)
                throw new MarshalException();
            switch (Data.GetByte(Position) & 0xe0)
            {
                case 0xe0:
                    UnmarshalByte();
                    return (int)UnmarshalUint();
                case 0xc0:
                    return (int)(UnmarshalUint() & ~0xc0000000);
                case 0xa0:
                case 0x80:
                    return UnmarshalUshort() & 0x7fff;
            }
            return UnmarshalByte();
        }

    }
}
