using System;
using System.Collections.Generic;

namespace Clones
{
    public class StackItem<T>
    {
        public T Value { get; set; }
        public StackItem<T> Prev { get; set; }
    }

    public class LinkedListStack<T>
    {
        private StackItem<T> _head;
        private StackItem<T> _tail;

        public LinkedListStack(LinkedListStack<T> other)
        {
            _head = other._head;
            _tail = other._tail;
        }

        public LinkedListStack()
        { }

        public bool IsEmpty => _head == null;

        public void Push(T value)
        {
            if (IsEmpty)
                _tail = _head = new StackItem<T> {Value = value, Prev = null};
            else
            {
                var item = new StackItem<T> {Value = value, Prev = _head};
                _head = item;
            }
        }

        public T Pop()
        {
            if (_head == null) throw new InvalidOperationException();
            var result = _head.Value;
            _head = _head.Prev;
            if (_head == null)
                _tail = null;
            return result;
        }

        public T Peek()
        {
            return _head.Value;
        }
    }

    public class CloneFeatures
    {
        private readonly string _cloneId;
        private readonly LinkedListStack<int> _stackLearned = new LinkedListStack<int>();
        private readonly LinkedListStack<int> _stackRollbacked = new LinkedListStack<int>();

        public CloneFeatures(string cloneId)
        {
            _cloneId = cloneId;
        }

        private CloneFeatures(string cloneId, LinkedListStack<int> stackLearned, LinkedListStack<int> stackRollbacked)
        {
            _cloneId = cloneId;
            _stackLearned = stackLearned;
            _stackRollbacked = stackRollbacked;
        }

        public void Learn(int programId)
        {
            _stackLearned.Push(programId);
        }

        public void Rollback()
        {
            _stackRollbacked.Push(_stackLearned.Pop());
        }

        public void Relearn()
        {
            _stackLearned.Push(_stackRollbacked.Pop());
        }

        public string Check()
        {
            return _stackLearned.IsEmpty ? "basic" : _stackLearned.Peek().ToString();
        }

        public CloneFeatures Clone() => new CloneFeatures(_cloneId,
            new LinkedListStack<int>(_stackLearned),
            new LinkedListStack<int>(_stackRollbacked));
    }

    public class CloneVersionSystem : ICloneVersionSystem
    {
        private readonly List<CloneFeatures> _listClones = new List<CloneFeatures>() {new CloneFeatures("basic")};
        private CloneFeatures _actualClone;

        public string Execute(string query)
        {
            var array = query.Split(' ');
            _actualClone = _listClones[int.Parse(array[1]) - 1];
            switch (array[0])
            {
                case "learn":
                    _actualClone.Learn(int.Parse(array[2]));
                    return null;
                case "rollback":
                    _actualClone.Rollback();
                    return null;
                case "relearn":
                    _actualClone.Relearn();
                    return null;
                case "clone":
                    _listClones.Add(_actualClone.Clone());
                    return null;
                case "check":
                    return _actualClone.Check();
            }
            return null;
        }
    }
}