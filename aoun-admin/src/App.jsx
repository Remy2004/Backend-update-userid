import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { useState } from 'react';
import Login from './pages/Login';
import DashboardLayout from './layouts/DashboardLayout';
import MainStats from './pages/MainStats';
import UsersManagement from './pages/UsersManagement';
import CharitiesManagement from './pages/CharitiesManagement';
import AdminsManagement from './pages/AdminsManagement';
import CasesManagement from './pages/CasesManagement';
import AppSettings from './pages/Settings';

function App() {
  const [isAuthenticated, setIsAuthenticated] = useState(!!localStorage.getItem('admin_token'));
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/login" element={!isAuthenticated ? <Login setAuth={setIsAuthenticated} /> : <Navigate to="/" />} />
        <Route path="/" element={isAuthenticated ? <DashboardLayout setAuth={setIsAuthenticated} /> : <Navigate to="/login" />}>
          <Route index element={<MainStats />} />
          <Route path="users" element={<UsersManagement />} />
          <Route path="charities" element={<CharitiesManagement />} />
          <Route path="admins" element={<AdminsManagement />} />
          <Route path="cases" element={<CasesManagement />} />
          <Route path="settings" element={<AppSettings />} />
        </Route>
      </Routes>
    </BrowserRouter>
  );
}
export default App;