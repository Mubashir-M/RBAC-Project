// usersListSlice.tsx

import {
  createSlice,
  createAsyncThunk,
  type PayloadAction,
} from "@reduxjs/toolkit";
import { AxiosError } from "axios";
import { type User, type Role } from "../types";

import api from "../api/axios";

interface UpdateUserRolePayload {
  userId: number;
  newRoleId: number;
  newRoleName: string;
}

interface RejectedUpdateUserRolePayload {
  payload: UpdateUserRolePayload; // The original thunk arguments
  error: string; // Your custom error message
}

interface UsersState {
  users: User[];
  loading: boolean;
  error: string | null;
  updatingUserIds: number[];
}

const initialState: UsersState = {
  users: [],
  loading: false,
  error: null,
  updatingUserIds: [],
};

// --- Helper function for AxiosError ---
function isAxiosError(error: unknown): error is AxiosError {
  return (error as AxiosError).isAxiosError !== undefined;
}

// --- fetchUsers Thunk  ---
export const fetchUsers = createAsyncThunk(
  "users/fetchUsers",
  async (_, thunkAPI) => {
    try {
      const response = await api.get("User/users");
      return response.data;
    } catch (error) {
      console.error(error);
      return thunkAPI.rejectWithValue("Failed to fetch users");
    }
  }
);

// --- updateUserRole Thunk  ---
export const updateUserRole = createAsyncThunk(
  "user/updateUserrole",
  async (payload: UpdateUserRolePayload, thunkAPI) => {
    try {
      await api.put(`User/users/${payload.userId}/role`, {
        roleId: payload.newRoleId,
        name: payload.newRoleName,
      });
      return payload;
    } catch (error: unknown) {
      console.error("API Error updating user role:", error);
      if (isAxiosError(error)) {
        const errorMessage =
          (error.response?.data as { error?: string })?.error ||
          "Failed to update user role on server.";
        return thunkAPI.rejectWithValue({
          payload, // The original payload (UpdateUserRolePayload)
          error: errorMessage,
        });
      }
      return thunkAPI.rejectWithValue({
        payload, // The original payload (UpdateUserRolePayload)
        error: "An unexpected error occurred during the update.",
      });
    }
  }
);

const userListSlice = createSlice({
  name: "usersList",
  initialState,
  reducers: {},
  extraReducers: (builder) => {
    builder
      // --- fetchUsers cases (assume correctly typed) ---
      .addCase(fetchUsers.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(fetchUsers.fulfilled, (state, action: PayloadAction<User[]>) => {
        state.loading = false;
        state.users = action.payload;
        state.error = null;
      })
      .addCase(fetchUsers.rejected, (state, action) => {
        // Adjusted to PayloadAction<string> if only error string is rejected
        state.loading = false;
        state.error = action.payload as string;
      })

      // --- updateUserRole.pending (unchanged, assumed correct) ---
      .addCase(
        updateUserRole.pending,
        (
          state,
          action: PayloadAction<void, string, { arg: UpdateUserRolePayload }>
        ) => {
          state.updatingUserIds.push(action.meta.arg.userId);
          const userIndex = state.users.findIndex(
            (user) => user.userId === action.meta.arg.userId
          );
          if (userIndex !== -1) {
            const updatedRoles: Role[] = [
              {
                roleId: action.meta.arg.newRoleId,
                name: action.meta.arg.newRoleName,
              },
            ];
            state.users[userIndex] = {
              ...state.users[userIndex],
              roles: updatedRoles,
            };
            state.error = null;
          }
        }
      )
      .addCase(
        updateUserRole.fulfilled,
        (state, action: PayloadAction<UpdateUserRolePayload>) => {
          state.updatingUserIds = state.updatingUserIds.filter(
            (id) => id !== action.payload.userId
          );
          state.error = null;
        }
      )
      .addCase(updateUserRole.rejected, (state, action) => {
        const rejectedPayload = action.payload as RejectedUpdateUserRolePayload;

        state.updatingUserIds = state.updatingUserIds.filter(
          (id) => id !== action.meta.arg.userId
        );

        state.error = rejectedPayload.error || "An unknown error occurred.";
      });
  },
});

export default userListSlice.reducer;
