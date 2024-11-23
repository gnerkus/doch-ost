import {Navigate, Route, Routes} from "react-router-dom";
import Layout from "./Layout.tsx";
import Login from "../auth/Login.tsx";
import RequireAuth from "./RequireAuth.tsx";
import Dashboard from "../dashboard/Dashboard.tsx";
import {useAuth} from "../auth/useAuth.ts";

function AppRoutes() {
    const auth = useAuth();

    return (
        <Routes>
            <Route element={<Layout auth={auth} />}>
                <Route path="/" element={<Navigate to="/dashboard" replace/>}/>
                <Route path="/login" element={<Login/>}/>
                <Route element={<RequireAuth session={auth.session} />}>
                    <Route path="/dashboard" element={<Dashboard />} />
                </Route>
                <Route
                    path="*"
                    element={<Navigate to="/dashboard" replace/>}
                />
            </Route>
        </Routes>
    )
}

export default AppRoutes