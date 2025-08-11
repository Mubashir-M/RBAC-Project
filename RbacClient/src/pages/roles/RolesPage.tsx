import React, { useState, useEffect } from "react";
import { useAppDispatch, useAppSelector } from "../../features/hooks";
import { type RootState } from "../../store/store";
import { createRole } from "../../features/roleSlice";
import "./RolesPage.css";

const RolesPage: React.FC = () => {
  const dispatch = useAppDispatch();
  const [search, setSearch] = useState("");
  const [showForm, setShowForm] = useState(false);
  const [name, setName] = useState("");
  const [description, setDescription] = useState("");

  const roles = useAppSelector((state: RootState) => state.roles.roles);

  const filteredRoles = roles.filter((role) =>
    `${role.name} ${role.roleId} ${role.description}`
      .toLocaleLowerCase()
      .includes(search.toLocaleLowerCase())
  );

  const handleCreateRole = (e: React.FormEvent) => {
    e.preventDefault();
    console.log("New role:", { name, description });
    dispatch(createRole({ name, description }))
      .unwrap()
      .then(() => {
        setShowForm(false);
        setName("");
        setDescription("");
      })
      .catch((error) => {
        console.error("Failed to create role: ", error);
      });
  };

  // Lock/unlock scrolling when modal is open
  useEffect(() => {
    if (showForm) {
      document.body.style.overflow = "hidden";
    } else {
      document.body.style.overflow = "";
    }
  }, [showForm]);

  return (
    <>
      <div className="roles-panel">
        <button className="create-button" onClick={() => setShowForm(true)}>
          + Create a Role...
        </button>

        <input
          type="text"
          placeholder="Search for roles..."
          value={search}
          onChange={(e) => setSearch(e.target.value)}
          className="search-input"
        />

        <div className="roles-container">
          <table className="roles-table">
            <thead>
              <tr>
                <th>Role ID</th>
                <th>Role Name</th>
                <th>Role Description</th>
              </tr>
            </thead>
            <tbody>
              {filteredRoles.length === 0 ? (
                <tr>
                  <td colSpan={5} className="no-roles">
                    No roles found.
                  </td>
                </tr>
              ) : (
                filteredRoles.map((role) => (
                  <tr key={role.roleId}>
                    <td>{role.roleId}</td>
                    <td>{role.name}</td>
                    <td>{role.description}</td>
                  </tr>
                ))
              )}
            </tbody>
          </table>
        </div>
      </div>

      {showForm && (
        <div className="modal-overlay" onClick={() => setShowForm(false)}>
          <div className="modal-content" onClick={(e) => e.stopPropagation()}>
            <h2>Create New Role</h2>
            <form onSubmit={handleCreateRole}>
              <input
                type="text"
                placeholder="Role Name"
                value={name}
                onChange={(e) => setName(e.target.value)}
                required
              />
              <textarea
                placeholder="Role Description"
                value={description}
                onChange={(e) => setDescription(e.target.value)}
                required
              />
              <div className="modal-actions">
                <button type="submit">Save</button>
                <button type="button" onClick={() => setShowForm(false)}>
                  Cancel
                </button>
              </div>
            </form>
          </div>
        </div>
      )}
    </>
  );
};

export default RolesPage;
