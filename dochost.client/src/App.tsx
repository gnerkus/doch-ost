import './App.css';
import {QueryClient, QueryClientProvider} from "@tanstack/react-query";
import Dashboard from "./dashboard/Dashboard.tsx";
import AuthProvider from "./auth/AuthContext.tsx";
import {Navigate, Route, Routes} from "react-router-dom";
import Login from "./auth/Login.tsx";
import Layout from "./layout/Layout.tsx";
import RequireAuth from "./layout/RequireAuth.tsx";

const queryClient = new QueryClient();

function App() {

    return (
        <AuthProvider>
            <QueryClientProvider client={queryClient}>
                <Routes>
                    <Route element={<Layout/>}>
                        <Route path="/" element={<Navigate to="/dashboard" replace/>}/>
                        <Route path="/login" element={<Login/>}/>
                        <Route
                            path="/dashboard"
                            element={
                                <RequireAuth>
                                    <Dashboard/>
                                </RequireAuth>
                            }
                        />
                        <Route
                            path="*"
                            element={<Navigate to="/dashboard" replace/>}
                        />
                    </Route>
                </Routes>
            </QueryClientProvider>
        </AuthProvider>
    )
}

export default App;