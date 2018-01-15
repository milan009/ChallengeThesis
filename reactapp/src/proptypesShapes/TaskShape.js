import PropTypes from "prop-types";

export const TaskShape = PropTypes.shape({
        id: PropTypes.number.isRequired,
        name: PropTypes.string.isRequired,

        description: PropTypes.string.isRequired,
        extraText: PropTypes.string,
    });