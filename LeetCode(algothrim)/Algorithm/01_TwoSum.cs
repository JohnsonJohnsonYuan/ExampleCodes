/*
Given an array of integers, return indices of the two numbers such that they add up to a specific target.
You may assume that each input would have exactly one solution, and you may not use the same element twice.

Example:
Given nums = [2, 7, 11, 15], target = 9,

Because nums[0] + nums[1] = 2 + 7 = 9,
return [0, 1].
*/

/// Put the answer for each number in Dictionary
/// E.g.: [3,2,4] , 6
/// [3], we push <3, 0> => means index 0 needs 3
/// [2], we push <4, 1> => means index 1 needs 4
/// [4], we found dictionary already contains it, which means previous element need [4] to get answer
namespace LeetCodeInCSharp {
    public class TwoSumSolution {
        public int[] TwoSum(int[] nums, int target) {
            var dict = new Dictionary<int, int>();
            
            for (int i = 0; i < nums.Length; i++)
            {
                if (dict.ContainsKey(nums[i])) {
                    var result = new int[2];
                    result[0] = dict[nums[i]];
                    result[1] = i;
                    return result;
                }
                else {
                    var exceed = target - nums[i];
                    if (!dict.ContainsKey(exceed))
                        dict.Add(exceed, i);
                }
            }

            return null;
        }
    }
}

