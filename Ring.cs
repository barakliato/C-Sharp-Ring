public class Ring<T>
    {
        private readonly T[] _buffer;
        
        private int _writeIndex = 0;
        private int _readIndex = 0;
        
        public ConcurrentRing(uint capacity)
        {
            _buffer = new T[capacity];
        }

        #region Write
        
        public void Write(T[] items)
        {
            int availableSpace = _buffer.Length - _writeIndex;
            if (availableSpace >= items.Length)
            {
                //there is enough space to write the incoming data
                WriteToBuffer(items);
            }
            else
            {
                //the items length is larger than the available space

                if (items.Length < _buffer.Length)
                {
                    //the buffer is large enough to hold the items
                    WriteInCycle(items, availableSpace);
                }
                else
                {
                    //the items length is larger than the entire buffer
                    //in this case we need to write the last items                    
                    FillBufferWithEndOfItems(items);
                }
            }
        }

        private void WriteToBuffer(T[] items)
        {
            Array.Copy(items, 0, _buffer, _writeIndex, items.Length);
            _writeIndex += items.Length;
            Interlocked.MemoryBarrier();
        }

        private void WriteInCycle(T[] items, int availableSpace)
        {
            //if the write index is NOT in the end of the buffer
            if (availableSpace > 0)
                Array.Copy(items, 0, _buffer, _writeIndex, availableSpace);

            //write the remaining items to the beggining of the buffer
            int secondWriteLength = items.Length - availableSpace;
            Array.Copy(items, availableSpace, _buffer, 0, secondWriteLength);

            //the writing index should begin where the second write ends
            _writeIndex = secondWriteLength;
            Interlocked.MemoryBarrier();
        }

        private void FillBufferWithEndOfItems(T[] items)
        {
            int startIndex = items.Length - _buffer.Length;
            Array.Copy(items, startIndex, _buffer, 0, _buffer.Length);
            _writeIndex = 0;
            Interlocked.MemoryBarrier();
        }

        #endregion

        #region Read

        public T[] Read(int length)
        {
            if (_readIndex == _writeIndex)
                return new T[0];
            int endIndex = _readIndex + length;
            if (_writeIndex > _readIndex && _writeIndex < endIndex)
            {
                //there isn't enough elements we can read up until the write index
                length = _writeIndex - _readIndex;
                endIndex = _writeIndex;
            }
            
            if (endIndex < _buffer.Length)
            {
                if (_writeIndex > _readIndex && _writeIndex < endIndex)
                {
                    //there isn't enough elements we can read up until the write index
                    length = _writeIndex - _readIndex;
                }

                //we can read directly on the buffer without any manipulations
                return ReadWithoutCycle(length, endIndex);
            }

            return ReadWithCycle(length);
        }

        private T[] ReadWithoutCycle(int length, int endIndex)
        {
            T[] result = new T[length];
            Array.Copy(_buffer, _readIndex, result, 0, length);
            _readIndex = endIndex;
            return result;
        }

        private T[] ReadWithCycle(int length)
        {
            var availableSpace = _buffer.Length - _readIndex;
            int secondReadLength = length - availableSpace;

            //in case the write index come before the end of the second read
            if (_writeIndex < secondReadLength)
                secondReadLength = _writeIndex;

            length = availableSpace + secondReadLength;
            T[] result = new T[length];

            Array.Copy(_buffer, _readIndex, result, 0, availableSpace);
            Array.Copy(_buffer, 0, result, availableSpace, secondReadLength);
            _readIndex = secondReadLength;
            return result;
        }

        #endregion
    }
