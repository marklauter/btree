using BTrees.Nodes;
using BTrees.Types;

namespace BTrees.Tests.Nodes
{
    public sealed class DataNodeTests
    {
        private readonly int nodeSize = 1024 * 2;

        [Fact]
        public void ContainsKey_Is_True_From_Newly_Constructed()
        {
            var key = (DbInt32)1;
            var value = (DbInt32)1;
            var node = new DataNode<DbInt32, DbInt32>(key, value);
            Assert.True(node.ContainsKey(key));
        }

        [Fact]
        public void ContainsKey_Is_True_After_Insert()
        {
            var key1 = (DbInt32)1;
            var value1 = (DbInt32)1;
            var node = new DataNode<DbInt32, DbInt32>(key1, value1);
            var key2 = (DbInt32)2;
            var value2 = (DbInt32)2;
            node.Insert(key2, value2);
            Assert.True(node.ContainsKey(key1));
            Assert.True(node.ContainsKey(key2));
        }

        [Fact]
        public void IsUnderflow_Is_True_From_Newly_Constructed()
        {
            var key = (DbInt32)1;
            var value = (DbInt32)1;
            var node = new DataNode<DbInt32, DbInt32>(key, value);
            Assert.True(node.IsUnderflow);
        }

        [Fact]
        public void IsOverflow_Is_False_From_Newly_Constructed()
        {
            var key = (DbInt32)1;
            var value = (DbInt32)1;
            var node = new DataNode<DbInt32, DbInt32>(key, value);
            Assert.False(node.IsOverflow);
        }

        [Fact]
        public void Read_Returns_Value_From_Newly_Constructed()
        {
            var key = (DbInt32)1;
            var value = (DbInt32)1;
            var node = new DataNode<DbInt32, DbInt32>(key, value);
            var actual = node.Read(key);
            Assert.Contains(value, actual);
            Assert.Contains(value, actual);
        }

        [Fact]
        public void Read_Returns_Inserted_Values_After_Insert()
        {
            var key1 = (DbInt32)1;
            var value1 = (DbInt32)1;
            var node = new DataNode<DbInt32, DbInt32>(key1, value1);
            var key2 = (DbInt32)2;
            var value2 = (DbInt32)2;
            node.Insert(key2, value2);
            Assert.True(node.ContainsKey(key1));
            Assert.True(node.ContainsKey(key2));
        }

        [Fact]
        public void Size_Is_Correct_From_Newly_Constructed()
        {
            var key = (DbInt32)1;
            var value = (DbInt32)1;
            var node = new DataNode<DbInt32, DbInt32>(key, value);
            Assert.Equal(DbInt32.Size * 2, node.Size);
        }

        [Fact]
        public void Size_Is_Correct_From_After_Insert()
        {
            var key = (DbInt32)1;
            var value = (DbInt32)1;
            var node = new DataNode<DbInt32, DbInt32>(key, value);
            var key2 = (DbInt32)2;
            var value2 = (DbInt32)2;
            node.Insert(key2, value2);
            Assert.Equal(DbInt32.Size * 4, node.Size);
        }

        [Fact]
        public void Length_Is_Correct_From_Newly_Constructed()
        {
            var key = (DbInt32)1;
            var value = (DbInt32)1;
            var node = new DataNode<DbInt32, DbInt32>(key, value);
            Assert.Equal(1, node.Length);
        }

        [Fact]
        public void Length_Is_Correct_From_After_Insert()
        {
            var key1 = (DbInt32)1;
            var value1 = (DbInt32)1;
            var node = new DataNode<DbInt32, DbInt32>(key1, value1);
            var key2 = (DbInt32)2;
            var value2 = (DbInt32)2;
            node.Insert(key2, value2);
            Assert.Equal(2, node.Length);
        }

        [Fact]
        public void Length_Is_Correct_From_After_Insert_With_Duplicate_Key()
        {
            var key1 = (DbInt32)1;
            var value1 = (DbInt32)1;
            var node = new DataNode<DbInt32, DbInt32>(key1, value1);
            var value2 = (DbInt32)2;
            node.Insert(key1, value2);
            Assert.Equal(1, node.Length);
        }

        [Fact]
        public void Count_Is_Correct_From_Newly_Constructed()
        {
            var key = (DbInt32)1;
            var value = (DbInt32)1;
            var node = new DataNode<DbInt32, DbInt32>(key, value);
            Assert.Equal(1, node.Count());
        }

        [Fact]
        public void Count_Is_Correct_From_After_Insert()
        {
            var key1 = (DbInt32)1;
            var value1 = (DbInt32)1;
            var node = new DataNode<DbInt32, DbInt32>(key1, value1);
            var key2 = (DbInt32)2;
            var value2 = (DbInt32)2;
            node.Insert(key2, value2);
            Assert.Equal(2, node.Count());
        }

        [Fact]
        public void Count_Is_Correct_From_After_Insert_With__Duplicate_Key()
        {
            var key1 = (DbInt32)1;
            var value1 = (DbInt32)1;
            var node = new DataNode<DbInt32, DbInt32>(key1, value1);
            var value2 = (DbInt32)2;
            node.Insert(key1, value2);
            Assert.Equal(2, node.Count());
        }

        [Fact]
        public void RightSibling_Is_Null_From_Newly_Constructed()
        {
            var key = (DbInt32)1;
            var value = (DbInt32)1;
            var node = new DataNode<DbInt32, DbInt32>(key, value);
            Assert.Null(node.RightSibling);
        }

        [Fact]
        public void Split_Returns_Right_Node()
        {
            var key1 = (DbInt32)1;
            var value1 = (DbInt32)1;
            var node = new DataNode<DbInt32, DbInt32>(key1, value1);
            var key2 = (DbInt32)2;
            var value2 = (DbInt32)2;
            node.Insert(key2, value2);
            var rightNode = node.Split();
            Assert.NotNull(rightNode);
        }

        [Fact]
        public void Split_Returns_Right_Node_That_Matches_RightSibling()
        {
            var key1 = (DbInt32)1;
            var value1 = (DbInt32)1;
            var node = new DataNode<DbInt32, DbInt32>(key1, value1);
            var key2 = (DbInt32)2;
            var value2 = (DbInt32)2;
            node.Insert(key2, value2);
            var rightNode = node.Split();
            Assert.Equal(rightNode, node.RightSibling);
        }

        [Fact]
        public void Split_RightSibling_ContainsKey()
        {
            var key1 = (DbInt32)1;
            var value1 = (DbInt32)1;
            var node = new DataNode<DbInt32, DbInt32>(key1, value1);
            var key2 = (DbInt32)2;
            var value2 = (DbInt32)2;
            node.Insert(key2, value2);
            var rightNode = node.Split();

            Assert.True(node.ContainsKey(key1));
            Assert.False(node.ContainsKey(key2));

            Assert.False(rightNode.ContainsKey(key1));
            Assert.True(rightNode.ContainsKey(key2));
        }

        [Fact]
        public void Split_Twice_RightSibling_Has_RightSibling()
        {
            var key1 = (DbInt32)1;
            var value1 = (DbInt32)1;
            var node1 = new DataNode<DbInt32, DbInt32>(key1, value1);

            var key2 = (DbInt32)2;
            var value2 = (DbInt32)2;
            node1.Insert(key2, value2);

            var key3 = (DbInt32)3;
            var value3 = (DbInt32)3;
            node1.Insert(key3, value3);

            var node2 = node1.Split();
            Assert.Equal(node2, node1.RightSibling);

            var node3 = node2.Split();
            Assert.Equal(node3, node2.RightSibling);

            Assert.Equal(node2, node1.RightSibling);
            Assert.NotEqual(node3, node1.RightSibling);
        }

        [Fact]
        public void Split_Twice_ContainsKey()
        {
            var key1 = (DbInt32)1;
            var value1 = (DbInt32)1;
            var node1 = new DataNode<DbInt32, DbInt32>(key1, value1);

            var key2 = (DbInt32)2;
            var value2 = (DbInt32)2;
            node1.Insert(key2, value2);

            var key3 = (DbInt32)3;
            var value3 = (DbInt32)3;
            node1.Insert(key3, value3);

            var node2 = node1.Split();
            var node3 = node2.Split();

            Assert.True(node1.ContainsKey(key1));
            Assert.False(node1.ContainsKey(key2));
            Assert.False(node1.ContainsKey(key3));

            Assert.False(node2.ContainsKey(key1));
            Assert.True(node2.ContainsKey(key2));
            Assert.False(node2.ContainsKey(key3));

            Assert.False(node3.ContainsKey(key1));
            Assert.False(node3.ContainsKey(key2));
            Assert.True(node3.ContainsKey(key3));
        }

        [Fact]
        public void Merge_Contains_All_Keys()
        {
            var key1 = (DbInt32)1;
            var value1 = (DbInt32)1;
            var node = new DataNode<DbInt32, DbInt32>(key1, value1);
            var key2 = (DbInt32)2;
            var value2 = (DbInt32)2;
            node.Insert(key2, value2);
            var rightNode = node.Split();
            node.Merge(rightNode);
            Assert.True(node.ContainsKey(key1));
            Assert.True(node.ContainsKey(key2));
        }

        [Fact]
        public void Merge_Reads_All_Keys()
        {
            var key1 = (DbInt32)1;
            var value1 = (DbInt32)1;
            var node = new DataNode<DbInt32, DbInt32>(key1, value1);
            var key2 = (DbInt32)2;
            var value2 = (DbInt32)2;
            node.Insert(key2, value2);
            var rightNode = node.Split();
            node.Merge(rightNode);
            Assert.Contains(value1, node.Read(key1));
            Assert.Contains(value2, node.Read(key2));
            var values = node
                .Read(key1, key2)
                .Select(kvp => kvp.Value);
            Assert.Contains(value1, values);
            Assert.Contains(value2, values);
        }

        [Fact]
        public void Merge_Sets_RightSibling()
        {
            var key1 = (DbInt32)1;
            var value1 = (DbInt32)1;
            var node1 = new DataNode<DbInt32, DbInt32>(key1, value1);

            var key2 = (DbInt32)2;
            var value2 = (DbInt32)2;
            node1.Insert(key2, value2);

            var key3 = (DbInt32)3;
            var value3 = (DbInt32)3;
            node1.Insert(key3, value3);

            var node2 = node1.Split();
            var node3 = node2.Split();

            node1.Merge(node2);
            Assert.Equal(node3, node1.RightSibling);
        }

        //        [Theory]
        //        [InlineData(1)]
        //        [InlineData(2)]
        //        [InlineData(5)]
        //        [InlineData(7)]
        //        [InlineData(13)]
        //        public void Multi_Threaded_Insert_Adds_All_Elements_In_Correct_Order(int rndSeed)
        //        {
        //            var size = 5000;
        //            var node = DataNode<int, int>.Empty(size);
        //            var rndArray = UniqueRandoms(rndSeed, size, size);

        //            var tasks = new Task[size];
        //            for (var i = 0; i < size; ++i)
        //            {
        //                var key = rndArray[i];
        //                var value = key;
        //                tasks[i] = Task.Run(() => node.Insert(key, value));
        //            }

        //            Task.WaitAll(tasks);
        //            Assert.Equal(size, node.Count);

        //            for (var i = 0; i < size; ++i)
        //            {
        //                var index = node.BinarySearch(rndArray[i]);
        //                Assert.Equal(rndArray[i], index);
        //            }

        //            for (var i = 0; i < size; ++i)
        //            {
        //                Assert.True(node.TryRead(rndArray[i], out var actualValue));
        //                Assert.Equal(rndArray[i], actualValue);
        //            }
        //        }
    }
}
