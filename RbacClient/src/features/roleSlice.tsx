import { createAsyncThunk, createSlice } from "@reduxjs/toolkit";
import api from "../api/axios";
import { type Role, type RawRole } from "../types";

interface RoleState {
  roles: Role[];
  loading: boolean;
  error: string | null;
}

const initialState: RoleState = {
  roles: [],
  loading: false,
  error: null,
};

export const fetchRoles = createAsyncThunk("roles/fetchRoles", async () => {
  const response = await api.get("Role/roles");
  console.log("here is roles:", response);
  return response.data;
});

const roleSlice = createSlice({
  name: "roles",
  initialState,
  reducers: {},
  extraReducers: (builder) => {
    builder
      .addCase(fetchRoles.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(fetchRoles.fulfilled, (state, action) => {
        state.loading = false;
        state.roles = action.payload.map((role: RawRole) => ({
          roleId: role.id,
          name: role.name,
        }));
      })
      .addCase(fetchRoles.rejected, (state, action) => {
        state.loading = false;
        state.error = action.error.message || "Failed to fetch roles.";
      });
  },
});

export default roleSlice.reducer;
