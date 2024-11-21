import {useAuth} from "../auth/AuthContext.tsx";
import {Navigate, useLocation} from "react-router-dom";

function RequireAuth({children}: { children: JSX.Element }) {
    const {session} = useAuth();
    const location = useLocation();

    if (!session) {
        return <Navigate to="/login" state={{from: location}} replace/>;
    }

    return children;
}

export default RequireAuth