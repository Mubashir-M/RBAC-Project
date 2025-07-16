import { createSlice, createAsyncThunk } from "@reduxjs/toolkit";
import { type User } from "../types";
import api from "../api/axios";

interface UsersState {
  users: User[];
  loading: boolean;
  error: string | null;
}

const initialState: UsersState = {
  users: [],
  loading: false,
  error: null,
};

export const updateUserRole = createAsyncThunk(
  "user/updateUserrole",
  async ({
    userId,
    newRoleId,
    newRoleName,
  }: {
    userId: number;
    newRoleId: number;
    newRoleName: string;
  }) => {
    await api.put(`User/users/${userId}/role`, {
      roleId: newRoleId,
      name: newRoleName,
    });
    return { userId, newRoleId };
  }
);

export const fetchUsers = createAsyncThunk(
  "users/fetchUsers",
  async (__reactRouterVersion, thunkAPI) => {
    try {
      const response = await api.get("User/users");
      return response.data;
    } catch (error) {
      console.error(error);
      return thunkAPI.rejectWithValue("Failed to fetch users");
    }
  }
);

const userListSlice = createSlice({
  name: "usersList",
  initialState,
  reducers: {},
  extraReducers: (builder) => {
    builder
      .addCase(fetchUsers.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(fetchUsers.fulfilled, (state, action) => {
        state.loading = false;
        state.users = action.payload; // Store fetched users
      })
      .addCase(fetchUsers.rejected, (state, action) => {
        state.loading = false;
        state.error = action.payload as string; // Handle errors
      })
      .addCase(updateUserRole.fulfilled, (state) => {
        state.loading = false;
        state.error = null;
      });
  },
});

export default userListSlice.reducer;
