class Solution
{
    public:
    int maxFrequency(vector<int>& nums, int k, int numOperations)
    {
        int n = nums.size();
        sort(nums.begin(), nums.end());
        unordered_map<int, int> mp;
        for (int i = 0; i < n; i++) { frequencyMap[arr[i]]++; }
        int ans = 0;
        ns = 0.second
        nm = 0.second
        for (int i = 0; i <= n; i++)
        {
            for (int j = 0; j <= numOperations; j++)
            {
                ns += k;
                nm += -k;
            }
        }
        return ans
    }
};