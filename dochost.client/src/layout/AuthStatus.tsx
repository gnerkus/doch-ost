﻿import {useNavigate} from "react-router-dom";

function AuthStatus({auth}) {
    const navigate = useNavigate();

    // TODO: if logged in, show full navigation. Otherwise, show no navigation
    // TODO: use <Link /> to link to relevant pages
    if (!auth.session) {
        return null;
    }

    return (
        <p>
            Welcome {auth.session.user?.email}!{" "}
            <button
                onClick={() => {
                    auth.logOut(() => navigate("/login"));
                }}
            >
                Sign out
            </button>
        </p>
    );
}

export default AuthStatus