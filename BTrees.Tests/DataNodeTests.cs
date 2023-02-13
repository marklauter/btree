using BTrees.Nodes;

namespace BTrees.Tests
{
    public sealed class DataNodeTests
    {
        private static int[] UniqueRandoms(int seed, int count, int maxValue)
        {
            var rnd = new Random(seed);
            var rndSet = new HashSet<int>(count);

            while (rndSet.Count < count)
            {
                var value = rnd.Next(maxValue);
                while (!rndSet.Add(value))
                {
                    value = rnd.Next(maxValue);
                }
            }

            return rndSet.ToArray();
        }

        [Fact]
        public void Empty_Node_Has_Correct_Size()
        {
            var size = 10;
            var node = DataNode<int, int>.Empty(size);
            Assert.Equal(size, node.Size);
        }

        [Fact]
        public void Empty_Node_Has_Correct_Count()
        {
            var size = 10;
            var node = DataNode<int, int>.Empty(size);
            Assert.Equal(0, node.Count);
        }

        [Fact]
        public void Empty_Node_IsEmpty()
        {
            var size = 10;
            var node = DataNode<int, int>.Empty(size);
            Assert.True(node.IsEmpty);
        }

        [Fact]
        public void Node_Contains_Key_After_Insert()
        {
            var size = 10;
            var key = 1;
            var value = 1;
            var node = DataNode<int, int>.Empty(size);
            node.Insert(key, value);

            Assert.True(node.ContainsKey(key));
        }

        [Fact]
        public void Node_Contains_Value_After_Insert()
        {
            var size = 10;
            var key = 1;
            var value = 1;
            var node = DataNode<int, int>.Empty(size);
            node.Insert(key, value);

            Assert.True(node.TryRead(key, out var actualValue));
            Assert.Equal(value, actualValue);
        }

        [Fact]
        public void Node_Contains_N_Elements_After_Inserts()
        {
            var size = 10;
            var node = DataNode<int, int>.Empty(size);
            for (var i = 0; i < size; ++i)
            {
                node.Insert(i, i);
            }

            Assert.Equal(size, node.Count);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(5)]
        [InlineData(7)]
        [InlineData(13)]
        public void Multi_Threaded_Insert_Adds_All_Elements_In_Correct_Order(int rndSeed)
        {
            var size = 5000;
            var node = DataNode<int, int>.Empty(size);
            var rndArray = UniqueRandoms(rndSeed, size, size);

            var tasks = new Task[size];
            for (var i = 0; i < size; ++i)
            {
                var key = rndArray[i];
                var value = key;
                tasks[i] = Task.Run(() => node.Insert(key, value));
            }

            Task.WaitAll(tasks);
            Assert.Equal(size, node.Count);

            for (var i = 0; i < size; ++i)
            {
                var index = node.BinarySearch(rndArray[i]);
                Assert.Equal(rndArray[i], index);
            }

            for (var i = 0; i < size; ++i)
            {
                Assert.True(node.TryRead(rndArray[i], out var actualValue));
                Assert.Equal(rndArray[i], actualValue);
            }
        }

        [Fact]
        public void Node_Contains_N_Elements_After_Delete()
        {
            var size = 10;
            var node = DataNode<int, int>.Empty(size);
            for (var i = 0; i < size; ++i)
            {
                node.Insert(i, i);
            }

            node.Delete(5);

            Assert.Equal(size - 1, node.Count);
        }

        [Fact]
        public void Node_Contains_Delete_Removes_Only_Appropriate_Element()
        {
            var size = 10;
            var node = DataNode<int, int>.Empty(size);
            for (var i = 0; i < size; ++i)
            {
                node.Insert(i, i);
            }

            node.Delete(5);

            Assert.False(node.ContainsKey(5));

            for (var i = 0; i < size; ++i)
            {
                if (i != 5)
                {
                    Assert.True(node.TryRead(i, out var value));
                    Assert.Equal(i, value);
                }
            }
        }

        [Fact]
        public void Node_Contains_New_Value_After_Update()
        {
            var size = 10;
            var key = 1;
            var value1 = 1;
            var value2 = 1;
            var node = DataNode<int, int>.Empty(size);
            node.Insert(key, value1);
            node.Update(key, value2);

            Assert.True(node.TryRead(key, out var actualValue));
            Assert.Equal(value2, actualValue);
        }

        [Fact]
        public void Split_Returns_New_Nodes()
        {
            var size = 10;
            var node = DataNode<int, int>.Empty(size);
            for (var i = 0; i < size; ++i)
            {
                node.Insert(i, i);
            }

            var (left, right, pivotKey) = node.Split();

            Assert.NotNull(left);
            Assert.Equal(size / 2, left.Count);

            Assert.NotNull(right);
            Assert.Equal(size / 2, right.Count);

            Assert.Equal(5, pivotKey);
        }

        [Fact]
        public void Split_Nodes_Contain_Correct_Key_Subsets()
        {
            var size = 10;
            var node = DataNode<int, int>.Empty(size);
            for (var i = 0; i < size; ++i)
            {
                node.Insert(i, i);
            }

            var (left, right, pivotKey) = node.Split();
            for (var i = 0; i < size / 2; ++i)
            {
                Assert.True(left.ContainsKey(i));
            }

            for (var i = pivotKey; i < pivotKey + size / 2; ++i)
            {
                Assert.True(right.ContainsKey(i));
            }
        }

        [Fact]
        public void Split_Nodes_Does_Not_Contain_Incorrect_Key_Subsets()
        {
            var size = 10;
            var node = DataNode<int, int>.Empty(size);
            for (var i = 0; i < size; ++i)
            {
                node.Insert(i, i);
            }

            var (left, right, pivotKey) = node.Split();
            for (var i = 0; i < size / 2; ++i)
            {
                Assert.False(right.ContainsKey(i));
            }

            for (var i = pivotKey; i < pivotKey + size / 2; ++i)
            {
                Assert.False(left.ContainsKey(i));
            }
        }

        [Fact]
        public void Merge_Left_Contains_All_Keys()
        {
            var size = 10;
            var node = DataNode<int, int>.Empty(size);
            for (var i = 0; i < size; ++i)
            {
                node.Insert(i, i);
            }

            var (left, right, _) = node.Split();
            var mergedNode = left.Merge(right);

            for (var i = 0; i < size; ++i)
            {
                Assert.True(mergedNode.ContainsKey(i));
            }
        }

        [Fact]
        public void Merge_Right_Contains_All_Keys()
        {
            var size = 10;
            var node = DataNode<int, int>.Empty(size);
            for (var i = 0; i < size; ++i)
            {
                node.Insert(i, i);
            }

            var (left, right, _) = node.Split();
            var mergedNode = right.Merge(left);

            for (var i = 0; i < size; ++i)
            {
                Assert.True(mergedNode.ContainsKey(i));
            }
        }
    }
}
