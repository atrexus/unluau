// Copyright (c) Valence. All Rights Reserved.
// Licensed under the Apache License, Version 2.0

using System.Text;

namespace Unluau.Format
{
    public class BytecodeReader
    {
        // ReSharper disable once InconsistentNaming
        private static readonly Encoding ASCII = Encoding.ASCII;

        private readonly BinaryReader _reader;

        /// <summary>
        /// The given stream.
        /// </summary>
        public Stream Stream { get; }

        /// <summary>
        /// The length of the stream.
        /// </summary>
        public long Length => Stream.Length;

        /// <summary>
        /// The current position of the stream.
        /// </summary>
        public long Position {
            get => _reader.BaseStream.Position;
            set => _reader.BaseStream.Position = value;
        }

        /// <summary>
        /// Initializes a new BytecodeReader reading from the specified byte array.
        /// </summary>
        public BytecodeReader(byte[] bytes) : this(new MemoryStream(bytes)) { }

        /// <summary>
        /// Initializes a new BytecodeReader reading from the specified stream.
        /// </summary>
        public BytecodeReader(Stream stream)
        {
            Stream = stream;
            _reader = new BinaryReader(stream);
        }

        /// <summary>
        /// Reads a one-byte boolean value.
        /// </summary>
        public bool ReadBoolean()
        {
            byte b = _reader.ReadByte();
            if (b > 1)
                throw new InvalidDataException($"ReadBoolean encountered non-boolean value: 0x{b:X2}");

            return Convert.ToBoolean(b);
        }

        /// <summary>
        /// Reads a 3 byte integer.
        /// </summary>
        public int ReadInt32()
        {
            return _reader.ReadInt32();
        }

        /// <summary>
        /// Reads a float.
        /// </summary>
        public float ReadSingle() 
        { 
            return _reader.ReadSingle();
        }

        /// <summary>
        /// Reads a 3 byte unsigned integer.
        /// </summary>
        public uint ReadUInt32()
        {
            return _reader.ReadUInt32();
        }

        /// <summary>
        /// Reads a compressed unsigned 32 bit integer.
        /// </summary>
        public uint ReadUInt32Compressed()
        {
            uint result = 0;
            int shift = 0;

            byte b;

            do
            {
                b = ReadByte();
                result |= (uint)((b & 127) << shift);
                shift += 7;
            } while ((b & 128) > 0);

            return result;
        }

        /// <summary>
        /// Reads a compressed 32-bit integer.
        /// </summary>
        public int ReadInt32Compressed()
        {
            return (int)ReadUInt32Compressed();
        }

        /// <summary>
        /// Reads a one-byte unsigned integer.
        /// </summary>
        public byte ReadByte()
        {
            return _reader.ReadByte();
        }

        /// <summary>
        /// Reads an array of one-byte unsigned integers from the specified offset without advancing the stream.
        /// </summary>
        public byte[] ReadBytes(int count)
        {
            byte[] result = _reader.ReadBytes(count);

            if (result.Length != count)
                throw new EndOfStreamException("Remaining size of stream was smaller than requested number of bytes.");
            return result;
        }

        /// <summary>
        /// Reads a two-byte signed integer.
        /// </summary>
        public short ReadInt16()
        {
            return _reader.ReadInt16();
        }

        /// <summary>
        /// Reads a two-byte unsigned integer.
        /// </summary>
        public ushort ReadUInt16()
        {
            return _reader.ReadUInt16();
        }

        /// <summary>
        /// Reads a 64-bit integer.
        /// </summary>
        public double ReadDouble()
        {
            return _reader.ReadDouble();
        }

        /// <summary>
        /// Reads a 64-bit unsigned integer.
        /// </summary>
        public double ReadU64()
        {
            return _reader.ReadUInt64();
        }

        /// <summary>
        /// Reads an ASCII string with the specified length in bytes.
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public string ReadASCII(int length)
        {
            byte[] bytes = ReadBytes(length);
            return ASCII.GetString(bytes);
        }

        public int Peek()
        {
            return _reader.PeekChar();
        }
    }
}
